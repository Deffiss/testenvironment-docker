using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using TestEnvironment.Docker.ContainerLifecycle;

namespace TestEnvironment.Docker.Containers.Mongo
{
    public class MongoContainerWaiter : BaseContainerWaiter<MongoContainer>
    {
        public MongoContainerWaiter()
        {
        }

        public MongoContainerWaiter(ILogger logger)
            : base(logger)
        {
        }

        protected override async Task<bool> PerformCheckAsync(MongoContainer container, CancellationToken cancellationToken)
        {
            await new MongoClient(container.GetConnectionString()).ListDatabasesAsync(cancellationToken);
            return true;
        }

        protected override bool IsRetryable(Exception exception) =>
            base.IsRetryable(exception) && !(exception is MongoAuthenticationException);
    }
}
