using System;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace TestEnvironment.Docker.Containers
{
    public class MssqlContainerWaiter : IContainerWaiter<MssqlContainer>
    {
        public async Task<(bool IsReady, string DebugMessage)> Wait(MssqlContainer container, CancellationToken cancellationToken)
        {
            if (container == null) new ArgumentNullException(nameof(container));

            var isAlive = false;
            string message = null;
            try
            {
                using (var connection = new SqlConnection(container.GetConnectionString()))
                using (var command = new SqlCommand("SELECT @@VERSION", connection))
                {
                    await command.Connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }

                isAlive = true;
            }
            catch (Exception ex) when (ex is InvalidOperationException || ex is NotSupportedException || ex is SqlException)
            {
                message = ex.Message;
            }

            return (isAlive, message);
        }

        public Task<(bool IsReady, string DebugMessage)> Wait(Container container, CancellationToken cancellationToken) => Wait((MssqlContainer)container, cancellationToken);
    }
}
