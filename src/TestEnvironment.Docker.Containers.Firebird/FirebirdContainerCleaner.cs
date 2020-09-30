using System;
using System.Threading;
using System.Threading.Tasks;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.Extensions.Logging;

namespace TestEnvironment.Docker.Containers.Firebird
{
    public class FirebirdContainerCleaner : IContainerCleaner<FirebirdContainer>
    {
        private readonly ILogger _logger;
        private readonly string _userName;

        public FirebirdContainerCleaner(ILogger logger, string userName)
        {
            _logger = logger;
            _userName = userName;
        }

        public async Task Cleanup(FirebirdContainer container, CancellationToken token = default)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            var cleanUpQuery = $"DROP OWNED BY {_userName}";

            using (var connection = new FbConnection(container.GetConnectionString()))
            using (var cleanUpCommand = new FbCommand(cleanUpQuery, connection))
            {
                try
                {
                    await connection.OpenAsync(token);
                    await cleanUpCommand.ExecuteNonQueryAsync(token);
                }
                catch (Exception ex)
                {
                    _logger?.LogError($"Firebird cleanup issue: {ex.Message}");
                }
            }
        }

        public Task Cleanup(Container container, CancellationToken token = default) => Cleanup((FirebirdContainer)container, token);
    }
}
