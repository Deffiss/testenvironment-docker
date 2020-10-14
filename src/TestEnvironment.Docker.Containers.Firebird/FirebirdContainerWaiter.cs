using System.Threading;
using System.Threading.Tasks;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.Extensions.Logging;

namespace TestEnvironment.Docker.Containers.Firebird
{
    public class FirebirdContainerWaiter : BaseContainerWaiter<FirebirdContainer>
    {
        public FirebirdContainerWaiter(ILogger logger)
            : base(logger)
        {
        }

        protected override async Task<bool> PerformCheck(FirebirdContainer container, CancellationToken cancellationToken)
        {
            using var connection = new FbConnection(container.GetConnectionString());
            using var command = new FbCommand("SELECT rdb$get_context('SYSTEM', 'ENGINE_VERSION') from rdb$database;", connection);

            await connection.OpenAsync(cancellationToken);
            await command.ExecuteNonQueryAsync(cancellationToken);

            return true;
        }
    }
}
