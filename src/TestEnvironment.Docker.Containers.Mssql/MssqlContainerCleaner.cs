using System;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TestEnvironment.Docker.ContainerLifecycle;

namespace TestEnvironment.Docker.Containers.Mssql
{
    public class MssqlContainerCleaner : IContainerCleaner<MssqlContainer>
    {
        private const string CleanupCommand = "EXEC sp_MSforeachdb " +
            @"'IF DB_ID(''?'') > 4 BEGIN
                ALTER DATABASE [?] SET SINGLE_USER WITH ROLLBACK IMMEDIATE
                DROP DATABASE [?]
            END'";

        private readonly ILogger? _logger;

        public MssqlContainerCleaner()
        {
        }

        public MssqlContainerCleaner(ILogger logger) =>
            _logger = logger;

        public async Task CleanupAsync(MssqlContainer container, CancellationToken cancellationToken = default)
        {
            using var connection = new SqlConnection(container.GetConnectionString());
            using var command = new SqlCommand(CleanupCommand, connection);
            try
            {
                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
            catch (SqlException e)
            {
                _logger?.LogInformation($"Cleanup issue: {e.Message}");
            }
        }

        public Task CleanupAsync(Container container, CancellationToken cancellationToken = default) => CleanupAsync((MssqlContainer)container, cancellationToken);
    }
}
