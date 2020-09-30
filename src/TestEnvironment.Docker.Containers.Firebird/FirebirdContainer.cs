using System.Collections.Generic;
using Docker.DotNet;
using Microsoft.Extensions.Logging;
using IP = System.Net.IPAddress;

namespace TestEnvironment.Docker.Containers.Firebird
{
    public class FirebirdContainer : Container
    {
        private readonly string _userName;
        private readonly string _password;

        public FirebirdContainer(
            DockerClient dockerClient,
            string name,
            string userName,
            string password,
            string imageName = "jacobalberty/firebird",
            string tag = "latest",
            IDictionary<string, string> environmentVariables = null,
            IDictionary<ushort, ushort> ports = null,
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
                  ports,
                  isDockerInDocker,
                  reuseContainer,
                  containerWaiter,
                  containerCleaner,
                  logger)
        {
            _userName = userName;
            _password = password;
        }

        public string GetConnectionString()
        {
            var host = IsDockerInDocker ? IPAddress : IP.Loopback.ToString();
            var port = IsDockerInDocker ? 3050 : Ports[3050];
            return $"DataSource={host};Port={port};User={_userName};Password={_password};Pooling=false;Database=SampleDatabase";
        }
    }
}
