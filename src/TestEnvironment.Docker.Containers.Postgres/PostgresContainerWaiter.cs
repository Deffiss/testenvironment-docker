using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TestEnvironment.Docker.Containers.Postgres
{
    public class PostgresContainerWaiter : BaseContainerWaiter<PostgresContainer>
    {
        public PostgresContainerWaiter(ILogger logger)
            : base(logger)
        {
        }

        protected override async Task<bool> PerformCheck(PostgresContainer container, CancellationToken cancellationToken)
        {
            try
            {
                Logger?.LogInformation($"Postgres: checking container state...");

                using var connection = new NpgsqlConnection(container.GetConnectionString());
                using var command = new NpgsqlCommand("select version()", connection);

                await connection.OpenAsync(cancellationToken);
                await command.ExecuteNonQueryAsync(cancellationToken);

                return true;
            }
            catch (Exception ex)
            {
                Logger?.LogError($"Postgres check failed with exception {ex.Message}");
                return false;
            }
        }
    }
}
