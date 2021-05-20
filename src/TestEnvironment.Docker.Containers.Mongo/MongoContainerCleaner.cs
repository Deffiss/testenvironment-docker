using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using TestEnvironment.Docker.ContainerLifecycle;

namespace TestEnvironment.Docker.Containers.Mongo
{
    public class MongoContainerCleaner
        : IContainerCleaner<MongoContainer>,
        IContainerCleaner<MongoSingleReplicaSetContainer>
    {
        private readonly ILogger? _logger;

        public MongoContainerCleaner()
        {
        }

        public MongoContainerCleaner(ILogger logger) =>
            _logger = logger;

        public Task CleanupAsync(MongoContainer container, CancellationToken cancellationToken = default) =>
            CleanupAsync((IMongoContainer)container, cancellationToken);

        public Task CleanupAsync(MongoSingleReplicaSetContainer container, CancellationToken cancellationToken = default) =>
            CleanupAsync((IMongoContainer)container, cancellationToken);

        public Task CleanupAsync(Container container, CancellationToken cancellationToken = default) =>
            CleanupAsync((IMongoContainer)container, cancellationToken);

        private async Task CleanupAsync(IMongoContainer container, CancellationToken cancellationToken)
        {
            var client = new MongoClient(container.GetConnectionString());
            var databaseNames = (await client.ListDatabasesAsync(cancellationToken))
                .ToList()
                .Select(x => x["name"].AsString);

            try
            {
                foreach (var databaseName in databaseNames)
                {
                    if (databaseName != "admin" && databaseName != "local")
                    {
                        await client.DropDatabaseAsync(databaseName, cancellationToken);
                    }
                }
            }
            catch (Exception e)
            {
                _logger?.LogInformation($"MongoDB cleanup issue: {e.Message}");
            }
        }
    }
}
