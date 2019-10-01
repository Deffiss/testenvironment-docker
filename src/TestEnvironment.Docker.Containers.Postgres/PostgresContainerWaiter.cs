using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TestEnvironment.Docker.Containers.Postgres
{
    public class PostgresContainerWaiter : IContainerWaiter<PostgresContainer>
    {
        private readonly ILogger _logger;

        public PostgresContainerWaiter(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<bool> Wait(PostgresContainer container, CancellationToken cancellationToken)
        {
            if (container == null) new ArgumentNullException(nameof(container));

            try
            {
                _logger?.LogInformation($"Postgres: checking container state...");
                using (var connection = new NpgsqlConnection(container.GetConnectionString()))
                using (var command = new NpgsqlCommand("select version()", connection))
                {
                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }

                return true;

            }
            catch (Exception ex)
            {
                _logger?.LogError($"Postgres check failed with exception {ex.Message}");
                return false;
            }
        }

        public Task<bool> Wait(Container container, CancellationToken cancellationToken) => Wait((PostgresContainer)container, cancellationToken);
    }
}
