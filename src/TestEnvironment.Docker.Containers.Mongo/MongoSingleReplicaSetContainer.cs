using System;
using System.Collections.Generic;
using Docker.DotNet;
using Microsoft.Extensions.Logging;

namespace TestEnvironment.Docker.Containers.Mongo
{
    public class MongoSingleReplicaSetContainer : Container, IMongoContainer
    {
        public MongoSingleReplicaSetContainer(
            DockerClient dockerClient,
            string name,
            string replicaSetName,
            string imageName,
            string tag = "latest",
            IDictionary<string, string> environmentVariables = null,
            bool isDockerInDocker = false,
            bool reuseContainer = false,
            IContainerWaiter containerWaiter = null,
            IContainerCleaner containerCleaner = null,
            ILogger logger = null)
            : base(
                dockerClient,
                name,
                imageName,
                tag,
                environmentVariables,
                new Dictionary<ushort, ushort> { { 27017, 27017 } },
                isDockerInDocker,
                reuseContainer,
                containerWaiter,
                containerCleaner,
                logger,
                new List<string> { "/usr/bin/mongod", "--bind_ip_all", "--replSet", replicaSetName },
                new MongoSingleReplicaSetContainerInitializer(replicaSetName))
        {
            if (string.IsNullOrWhiteSpace(replicaSetName))
            {
                throw new ArgumentException("The value must be specified", nameof(replicaSetName));
            }
        }

        public string GetDirectNodeConnectionString()
        {
            var hostname = IsDockerInDocker ? IPAddress : "localhost";
            var port = IsDockerInDocker ? 27017 : Ports[27017];

            return $@"mongodb://{hostname}:{port}/?connect=direct";
        }

        public string GetConnectionString()
        {
            var hostname = IsDockerInDocker ? IPAddress : "localhost";
            var port = IsDockerInDocker ? 27017 : Ports[27017];

            return $@"mongodb://{hostname}:{port}/?replicaSet=rs0";
        }
    }
}
