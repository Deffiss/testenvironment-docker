using System;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using TestEnvironment.Docker.ContainerLifecycle;

namespace TestEnvironment.Docker.Containers.Mongo
{
    public class MongoSingleReplicaSetContainerInitializer : IContainerInitializer<MongoSingleReplicaSetContainer>
    {
        public async Task<bool> InitializeAsync(
            MongoSingleReplicaSetContainer container,
            CancellationToken cancellationToken)
        {
            var mongoClient = new MongoClient(container.GetDirectNodeConnectionString());

            if (await IsInitialized(container, mongoClient, cancellationToken))
            {
                return true;
            }

            await mongoClient.GetDatabase("admin").RunCommandAsync(
                new BsonDocumentCommand<BsonDocument>(new BsonDocument
                {
                    {
                        "replSetInitiate",
                        new BsonDocument
                        {
                            { "_id", container.ReplicaSetName },
                            {
                                "members",
                                new BsonArray
                                {
                                    new BsonDocument
                                    {
                                        { "_id", 0 },
                                        {
                                            "host",
                                            mongoClient.Settings.Server.ToString()
                                        }
                                    }
                                }
                            }
                        }
                    }
                }),
                cancellationToken: cancellationToken);

            return true;
        }

        public Task<bool> InitializeAsync(Container container, CancellationToken cancellationToken) =>
            InitializeAsync((MongoSingleReplicaSetContainer)container, cancellationToken);

        private async Task<bool> IsInitialized(MongoSingleReplicaSetContainer container, IMongoClient mongoClient, CancellationToken cancellationToken)
        {
            try
            {
                var configuration = await mongoClient.GetDatabase("admin")
                    .RunCommandAsync(
                        new BsonDocumentCommand<BsonDocument>(new BsonDocument { { "replSetGetConfig", 1 } }),
                        cancellationToken: cancellationToken);

                return configuration["config"]["_id"].AsString == container.ReplicaSetName &&
                       configuration["config"]["members"].AsBsonArray.Count == 1 &&
                       configuration["config"]["members"].AsBsonArray[0]["_id"] == 0 &&
                       configuration["config"]["members"].AsBsonArray[0]["host"] ==
                       mongoClient.Settings.Server.ToString();
            }
            catch (MongoCommandException exception) when (exception.Code == 94 /*"NotYetInitialized"*/)
            {
                return false;
            }
        }
    }
}
