using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace TestEnvironment.Docker.Containers.Mongo
{
    public class MongoContainerWaiter : BaseContainerWaiter<MongoContainer>
    {
        public MongoContainerWaiter(ILogger logger = null)
            : base(logger)
        {
        }

        protected override async Task<bool> PerformCheck(MongoContainer container, CancellationToken cancellationToken)
        {
            try
            {
                Logger?.LogInformation($"MongoDB: checking container state...");
                
                await new MongoClient(container.GetConnectionString()).ListDatabasesAsync(cancellationToken);
                
                Logger?.LogInformation($"MongoDB: container is Up!");
                return true;
            }
            catch (Exception e)
            {
                Logger?.LogError($"MongoDB: check failed with exception {e.Message}");
                return false;
            }
        }
    }
}