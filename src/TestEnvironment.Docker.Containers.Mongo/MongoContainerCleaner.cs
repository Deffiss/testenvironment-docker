using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace TestEnvironment.Docker.Containers.Mongo
{
    public class MongoContainerCleaner : IContainerCleaner<MongoContainer>
    {
        private readonly ILogger _logger;

        public MongoContainerCleaner(ILogger logger = null)
        {
            _logger = logger;
        }

        public async Task Cleanup(MongoContainer container, CancellationToken token = default)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            var client = new MongoClient(container.GetConnectionString());
            var databaseNames = (await client.ListDatabaseNamesAsync(token)).ToList();
            try
            {
                foreach (var databaseName in databaseNames)
                {
                    if (databaseName != "admin" && databaseName != "local")
                    {
                        await client.DropDatabaseAsync(databaseName, token);
                    }
                }
            }
            catch (Exception e)
            {
                _logger?.LogInformation($"MongoDB cleanup issue: {e.Message}");
            }
        }

        public Task Cleanup(Container container, CancellationToken token = default) =>
            Cleanup((MongoContainer)container, token);
    }
}