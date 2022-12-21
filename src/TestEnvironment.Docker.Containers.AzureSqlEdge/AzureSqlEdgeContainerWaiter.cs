using System;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TestEnvironment.Docker.ContainerLifecycle;

namespace TestEnvironment.Docker.Containers.AzureSqlEdge
{
    public class AzureSqlEdgeContainerWaiter : BaseContainerWaiter<AzureSqlEdgeContainer>
    {
        public AzureSqlEdgeContainerWaiter()
        {
        }

        public AzureSqlEdgeContainerWaiter(ILogger logger)
            : base(logger)
        {
        }

        protected override async Task<bool> PerformCheckAsync(AzureSqlEdgeContainer container, CancellationToken cancellationToken)
        {
            await using var connection = new SqlConnection(container.GetConnectionString());
            await using var command = new SqlCommand("SELECT @@VERSION", connection);

            await connection.OpenAsync(cancellationToken);
            await command.ExecuteNonQueryAsync(cancellationToken);

            return true;
        }

        protected override bool IsRetryable(Exception exception) =>
            exception is InvalidOperationException or NotSupportedException or SqlException;
    }
}