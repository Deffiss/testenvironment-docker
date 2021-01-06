using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;

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
            using var command = new OracleCommand("SELECT * FROM V$VERSION", connection);

            await connection.OpenAsync(cancellationToken);
            await new OracleCommand("alter session set time_zone = '+2:00'", connection).ExecuteNonQueryAsync();

            OracleGlobalization info = connection.GetSessionInfo();
            info.TimeZone = "UTC";
            connection.SetSessionInfo(info);

            await command.ExecuteNonQueryAsync(cancellationToken);

            return true;
        }
    }
}
