using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace TestEnvironment.Docker.Containers.Oracle
{
    public class OracleContainerWaiter : BaseContainerWaiter<OracleContainer>
    {
        public OracleContainerWaiter(ILogger logger)
            : base(logger)
        {
        }

        protected override async Task<bool> PerformCheck(OracleContainer container, CancellationToken cancellationToken)
        {
            using var connection = new OracleConnection(container.GetConnectionString());
            connection.Open();

            try
            {
                Logger.LogInformation($"Timezone initial: {new OracleCommand("SELECT SESSIONTIMEZONE FROM DUAL", connection).ExecuteScalar()}");
            }
            catch
            {
            }

            try
            {
                Logger.LogInformation("Altering session timezone");
                new OracleCommand("alter session set time_zone = '+00:00'", connection).ExecuteNonQuery();
            }
            catch
            {
            }

            try
            {
                Logger.LogInformation($"Timezone after: {new OracleCommand("SELECT SESSIONTIMEZONE FROM DUAL", connection).ExecuteScalar()}");
            }
            catch
            {
            }

            using var command = new OracleCommand("SELECT * FROM V$VERSION", connection);

            OracleGlobalization info = connection.GetSessionInfo();

            // info.TimeZone = "America/New_York";
            info.TimeZone = string.Format("{0}:{1}", TimeZoneInfo.Local.BaseUtcOffset.Hours, TimeZoneInfo.Local.BaseUtcOffset.Minutes);

            connection.SetSessionInfo(info);

            await command.ExecuteNonQueryAsync(cancellationToken);

            return true;
        }
    }
}
