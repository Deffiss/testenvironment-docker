using System.Collections.Generic;
using Docker.DotNet;
using Microsoft.Extensions.Logging;

namespace TestEnvironment.Docker.Containers.Mongo
{
    public class MongoSingleReplicaSetContainer : Container
    {
        public MongoSingleReplicaSetContainer(
            DockerClient dockerClient,
            string name,
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
                new List<string> { "/usr/bin/mongod", "--bind_ip_all", "--replSet", "rs0" },
                new MongoSingleReplicaSetContainerInitializer())
        {
        }

        public string GetDirectNodeConnectionString()
        {
            var hostname = IsDockerInDocker ? IPAddress : "localhost";
            var port = IsDockerInDocker ? 27017 : Ports[27017];

            return $@"mongodb://{hostname}:{port}/?connect=direct";

            // return string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(UserPassword)
            //    ? $@"mongodb://{hostname}:{port}/?connect=direct"
            //    : $@"mongodb://{UserName}:{UserPassword}@{hostname}:{port}/?connect=direct";
        }

        public string GetConnectionString()
        {
            var hostname = IsDockerInDocker ? IPAddress : "localhost";
            var port = IsDockerInDocker ? 27017 : Ports[27017];

            return $@"mongodb://{hostname}:{port}/?replicaSet=rs0";

            // return string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(UserPassword)
            //     ? $@"mongodb://{hostname}:{port}/?replicaSet=rs0"
            //     : $@"mongodb://{UserName}:{UserPassword}@{hostname}:{port}/?authSource=admin&replicaSet=rs0";
        }
    }
}
