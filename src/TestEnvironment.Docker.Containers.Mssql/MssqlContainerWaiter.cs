using Microsoft.Extensions.Logging;
using System;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace TestEnvironment.Docker.Containers.Mssql
{
    public class MssqlContainerWaiter : BaseContainerWaiter<MssqlContainer>
    {
        public MssqlContainerWaiter(ILogger logger = null)
            : base(logger)
        {
        }

        protected override async Task<bool> PerformCheck(MssqlContainer container, CancellationToken cancellationToken)
        {
            try
            {
                using var connection = new SqlConnection(container.GetConnectionString());
                using var command = new SqlCommand("SELECT @@VERSION", connection);
                
                await connection.OpenAsync(cancellationToken);
                await command.ExecuteNonQueryAsync(cancellationToken);

                return true;
            }
            catch (Exception ex) when (ex is InvalidOperationException || ex is NotSupportedException || ex is SqlException)
            {
                Logger?.LogDebug(ex.Message);
            }

            return false;
        }
    }
}
