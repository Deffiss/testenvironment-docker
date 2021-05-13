using System.Collections.Generic;
using Docker.DotNet;
using Microsoft.Extensions.Logging;
using TestEnvironment.Docker.ContainerOperations;
using TestEnvironment.Docker.ImageOperations;
using IP = System.Net.IPAddress;

namespace TestEnvironment.Docker.Containers.Mssql
{
    public sealed class MssqlContainer : Container
    {
        private readonly MssqlContainerParameters _parameters;

        public MssqlContainer(MssqlContainerParameters containerParameters)
            : base(containerParameters) =>
            _parameters = containerParameters;

        public MssqlContainer(MssqlContainerParameters containerParameters, IDockerClient dockerClient)
            : base(containerParameters, dockerClient) =>
            _parameters = containerParameters;

        public MssqlContainer(MssqlContainerParameters containerParameters, IDockerClient dockerClient, ILogger? logger)
            : base(containerParameters, dockerClient, logger) =>
            _parameters = containerParameters;

        public MssqlContainer(MssqlContainerParameters containerParameters, IContainerApi containerApi, ImageApi imageApi, ILogger? logger)
            : base(containerParameters, containerApi, imageApi, logger) =>
            _parameters = containerParameters;

        public string GetConnectionString() =>
            $"Data Source={(IsDockerInDocker ? IPAddress : IP.Loopback.ToString())}, {(IsDockerInDocker ? 1433 : Ports![1433])}; UID=sa; pwd={_parameters.SAPassword};";
    }
}
