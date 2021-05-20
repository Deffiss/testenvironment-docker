using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Npgsql;
using TestEnvironment.Docker.ContainerLifecycle;

namespace TestEnvironment.Docker.Containers.Postgres
{
    public class PostgresContainerCleaner : IContainerCleaner<PostgresContainer>
    {
        private readonly ILogger? _logger;

        public PostgresContainerCleaner()
        {
        }

        public PostgresContainerCleaner(ILogger logger) =>
            _logger = logger;

        public async Task CleanupAsync(PostgresContainer container, CancellationToken token = default)
        {
            var cleanUpQuery = $"DROP OWNED BY {container.UserName}";

            using var connection = new NpgsqlConnection(container.GetConnectionString());
            using var cleanUpCommand = new NpgsqlCommand(cleanUpQuery, connection);
            try
            {
                await connection.OpenAsync();
                await cleanUpCommand.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                _logger?.LogError($"Postgres cleanup issue: {ex.Message}");
            }
        }

        public Task CleanupAsync(Container container, CancellationToken token = default) => CleanupAsync((PostgresContainer)container, token);
    }
}
