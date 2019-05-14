using System;
using System.Threading;
using System.Threading.Tasks;
using TestEnvironment.Docker;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace TestEnvironment.Docker.Containers.MariaDB
{
    public class MariaDBContainerWaiter : IContainerWaiter<MariaDBContainer>
    {
        private readonly ILogger _logger;

        public MariaDBContainerWaiter(ILogger logger = null)
        {
            _logger = logger;
        }

        public async Task<bool> Wait(MariaDBContainer container, CancellationToken cancellationToken)
        {
            if (container == null) new ArgumentNullException(nameof(container));

            try
            {
                using (var connection = new MySqlConnection(container.GetConnectionString()))
                using (var command = new MySqlCommand("select @@version", connection))
                {
                    await command.Connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }

                return true;
            }
            catch (Exception ex) when (ex is InvalidOperationException || ex is NotSupportedException || ex is MySqlException)
            {
                _logger?.LogDebug(ex.Message);
            }

            return false;
        }

        public Task<bool> Wait(Container container, CancellationToken cancellationToken) => Wait((MariaDBContainer)container, cancellationToken);
    }
}
