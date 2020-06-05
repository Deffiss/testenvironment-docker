using System.Collections.Generic;
using Docker.DotNet;
using Microsoft.Extensions.Logging;

namespace TestEnvironment.Docker.Containers.Mongo
{
    public class MongoContainer : Container
    {
        public MongoContainer(
            DockerClient dockerClient,
            string name,
            string userName,
            string userPassword,
            string imageName,
            string tag = "latest",
            IDictionary<string, string> environmentVariables = null,
            IDictionary<ushort, ushort> ports = null,
            bool isDockerInDocker = false,
            bool reuseContainer = false,
            IContainerWaiter containerWaiter = null,
            IContainerCleaner containerCleaner = null,
            ILogger logger = null,
            IList<string> entrypoint = null,
            IContainerInitializer containerInitializer = null)
            : base(dockerClient, name, imageName, tag, environmentVariables, ports, isDockerInDocker, reuseContainer, containerWaiter, containerCleaner, logger, entrypoint, containerInitializer)
        {
            UserName = userName;
            UserPassword = userPassword;
        }

        protected string UserName { get; }

        protected string UserPassword { get; }

        public virtual string GetConnectionString()
        {
            var hostname = IsDockerInDocker ? IPAddress : "localhost";
            var port = IsDockerInDocker ? 27017 : Ports[27017];

            return string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(UserPassword)
                ? $@"mongodb://{hostname}:{port}"
                : $@"mongodb://{UserName}:{UserPassword}@{hostname}:{port}";
        }
    }
}