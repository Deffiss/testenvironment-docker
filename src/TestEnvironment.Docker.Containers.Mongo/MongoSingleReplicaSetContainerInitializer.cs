using System;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace TestEnvironment.Docker.Containers.Mongo
{
    public class MongoSingleReplicaSetContainerInitializer : IContainerInitializer<MongoSingleReplicaSetContainer>
    {
        public async Task<bool> Initialize(
            MongoSingleReplicaSetContainer container,
            CancellationToken cancellationToken)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            var mongoClient = new MongoClient(container.GetDirectNodeConnectionString());

            await mongoClient.GetDatabase("admin").RunCommandAsync(
                new BsonDocumentCommand<BsonDocument>(new BsonDocument
                {
                    {
                        "replSetInitiate",
                        new BsonDocument
                        {
                            { "_id", "rs0" },
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
                }), cancellationToken: cancellationToken);

            return true;
        }

        public Task<bool> Initialize(Container container, CancellationToken cancellationToken) =>
            Initialize(container as MongoSingleReplicaSetContainer, cancellationToken);
    }
}
