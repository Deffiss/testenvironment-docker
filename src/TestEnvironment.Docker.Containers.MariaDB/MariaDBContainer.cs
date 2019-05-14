using System;
using System.Collections.Generic;
using System.Text;
using Docker.DotNet;
using Microsoft.Extensions.Logging;
using TestEnvironment.Docker;

namespace TestEnvironment.Docker.Containers.MariaDB
{
    public class MariaDBContainer : Container
    {
        private readonly string _rootPassowrd;

        public MariaDBContainer(
            DockerClient dockerClient,
            string name,
            string rootPassword,
            string imageName = "mariadb",
            string tag = "latest",
            IDictionary<string, string> environmentVariables = null,
            IDictionary<ushort, ushort> ports = null,
            bool isDockerInDocker = false,
            bool reuseContainer = false,
            ILogger logger = null)
            : base(dockerClient, name, imageName, tag, new Dictionary<string, string> { ["MYSQL_ROOT_PASSWORD"] = rootPassword }.MergeDictionaries(environmentVariables), ports, isDockerInDocker, reuseContainer, new MariaDBContainerWaiter(logger), new MariaDBContainerCleaner(logger), logger)
        {
            _rootPassowrd = rootPassword;
        }

        public string GetConnectionString() =>
            $"server={(IsDockerInDocker ? IPAddress : "localhost")};user=root;password={_rootPassowrd};port={(IsDockerInDocker ? 3306 : Ports[3306])};allow user variables=true";
    }
}
