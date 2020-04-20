using System;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace TestEnvironment.Docker.Containers.Mssql
{
    public class MssqlContainerWaiter : BaseContainerWaiter<MssqlContainer>
    {
        public MssqlContainerWaiter(ILogger logger = null)
            : base(logger)
        {
        }

        protected override async Task<bool> PerformCheck(MssqlContainer container, CancellationToken cancellationToken)
        {
            using var connection = new SqlConnection(container.GetConnectionString());
            using var command = new SqlCommand("SELECT @@VERSION", connection);

            await connection.OpenAsync(cancellationToken);
            await command.ExecuteNonQueryAsync(cancellationToken);

            return true;
        }

        protected override bool IsRetryable(Exception exception) =>
            exception is InvalidOperationException || exception is NotSupportedException || exception is SqlException;
    }
}
