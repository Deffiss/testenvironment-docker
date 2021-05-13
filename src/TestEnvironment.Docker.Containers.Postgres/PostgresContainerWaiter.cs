using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Npgsql;
using TestEnvironment.Docker.ContainerLifecycle;

namespace TestEnvironment.Docker.Containers.Postgres
{
    public class PostgresContainerWaiter : BaseContainerWaiter<PostgresContainer>
    {
        public PostgresContainerWaiter()
        {
        }

        public PostgresContainerWaiter(ILogger logger)
            : base(logger)
        {
        }

        protected override async Task<bool> PerformCheckAsync(PostgresContainer container, CancellationToken cancellationToken)
        {
            using var connection = new NpgsqlConnection(container.GetConnectionString());
            using var command = new NpgsqlCommand("select version()", connection);

            await connection.OpenAsync(cancellationToken);
            await command.ExecuteNonQueryAsync(cancellationToken);

            return true;
        }
    }
}
