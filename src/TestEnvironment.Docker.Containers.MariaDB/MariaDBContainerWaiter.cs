using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace TestEnvironment.Docker.Containers.MariaDB
{
    public class MariaDBContainerWaiter : BaseContainerWaiter<MariaDBContainer>
    {
        public MariaDBContainerWaiter(ILogger logger = null)
            : base(logger)
        {
        }

        protected override async Task<bool> PerformCheck(MariaDBContainer container, CancellationToken cancellationToken)
        {
            try
            {
                // ReSharper disable UseAwaitUsing
                using var connection = new MySqlConnection(container.GetConnectionString());
                using var command = new MySqlCommand("select @@version", connection);
                // ReSharper restore UseAwaitUsing
                
                await command.Connection.OpenAsync(cancellationToken);
                await command.ExecuteNonQueryAsync(cancellationToken);

                return true;
            }
            catch (Exception ex) when (ex is InvalidOperationException || ex is NotSupportedException || ex is MySqlException)
            {
                Logger?.LogDebug(ex.Message);
            }

            return false;
        }

    }
}
