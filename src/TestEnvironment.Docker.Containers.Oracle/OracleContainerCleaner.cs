using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;

namespace TestEnvironment.Docker.Containers.Oracle
{
    public class OracleContainerCleaner : IContainerCleaner<OracleContainer>
    {
        private readonly ILogger _logger;
        private readonly string _userName;

        public OracleContainerCleaner(ILogger logger, string userName)
        {
            _logger = logger;
            _userName = userName;
        }

        public async Task Cleanup(OracleContainer container, CancellationToken token = default)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            var cleanUpQuery = $"DROP OWNED BY {_userName}";

            using (var connection = new OracleConnection(container.GetConnectionString()))
            using (var cleanUpCommand = new OracleCommand(cleanUpQuery, connection))
            {
                try
                {
                    await connection.OpenAsync(token);
                    await cleanUpCommand.ExecuteNonQueryAsync(token);
                }
                catch (Exception ex)
                {
                    _logger?.LogError($"Oracle cleanup issue: {ex.Message}");
                }
            }
        }

        public Task Cleanup(Container container, CancellationToken token = default) => Cleanup((OracleContainer)container, token);
    }
}
