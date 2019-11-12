using Microsoft.Extensions.Logging;
using System;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace TestEnvironment.Docker.Containers.Mssql
{
    public class MssqlContainerWaiter : IContainerWaiter<MssqlContainer>
    {
        private readonly ILogger _logger;

        public MssqlContainerWaiter(ILogger logger = null)
        {
            _logger = logger;
        }

        public async Task<bool> Wait(MssqlContainer container, CancellationToken cancellationToken)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            try
            {
                using (var connection = new SqlConnection(container.GetConnectionString()))
                using (var command = new SqlCommand("SELECT @@VERSION", connection))
                {
                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }

                return true;
            }
            catch (Exception ex) when (ex is InvalidOperationException || ex is NotSupportedException || ex is SqlException)
            {
                _logger?.LogDebug(ex.Message);
            }

            return false;
        }

        public Task<bool> Wait(Container container, CancellationToken cancellationToken) => Wait((MssqlContainer)container, cancellationToken);
    }
}
