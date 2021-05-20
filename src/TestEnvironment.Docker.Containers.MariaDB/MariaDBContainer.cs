using System.Collections.Generic;
using Docker.DotNet;
using Microsoft.Extensions.Logging;
using TestEnvironment.Docker.ContainerOperations;
using TestEnvironment.Docker.ImageOperations;
using IP = System.Net.IPAddress;

namespace TestEnvironment.Docker.Containers.MariaDB
{
    public class MariaDBContainer : Container
    {
        private readonly MariaDBContainerParameters _parameters;

        public MariaDBContainer(MariaDBContainerParameters containerParameters)
            : base(containerParameters) =>
            _parameters = containerParameters;

        public MariaDBContainer(MariaDBContainerParameters containerParameters, IDockerClient dockerClient)
            : base(containerParameters, dockerClient) =>
            _parameters = containerParameters;

        public MariaDBContainer(MariaDBContainerParameters containerParameters, IDockerClient dockerClient, ILogger? logger)
            : base(containerParameters, dockerClient, logger) =>
            _parameters = containerParameters;

        public MariaDBContainer(MariaDBContainerParameters containerParameters, IContainerApi containerApi, ImageApi imageApi, ILogger? logger)
            : base(containerParameters, containerApi, imageApi, logger) =>
            _parameters = containerParameters;

        public string GetConnectionString() =>
            $"server={(IsDockerInDocker ? IPAddress : IP.Loopback.ToString())};user=root;password={_parameters.RootPassword};port={(IsDockerInDocker ? 3306 : Ports![3306])};allow user variables=true";
    }
}
