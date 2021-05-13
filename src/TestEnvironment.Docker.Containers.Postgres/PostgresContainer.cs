using System.Collections.Generic;
using Docker.DotNet;
using Microsoft.Extensions.Logging;
using TestEnvironment.Docker.ContainerOperations;
using TestEnvironment.Docker.ImageOperations;
using IP = System.Net.IPAddress;

namespace TestEnvironment.Docker.Containers.Postgres
{
    public class PostgresContainer : Container
    {
        private readonly PostgresContainerParameters _parameters;

        public string UserName => _parameters.UserName;

#pragma warning disable SA1201 // Elements should appear in the correct order
        public PostgresContainer(PostgresContainerParameters containerParameters)
#pragma warning restore SA1201 // Elements should appear in the correct order
            : base(containerParameters) =>
            _parameters = containerParameters;

        public PostgresContainer(PostgresContainerParameters containerParameters, IDockerClient dockerClient)
            : base(containerParameters, dockerClient) =>
            _parameters = containerParameters;

        public PostgresContainer(PostgresContainerParameters containerParameters, IDockerClient dockerClient, ILogger? logger)
            : base(containerParameters, dockerClient, logger) =>
            _parameters = containerParameters;

        public PostgresContainer(PostgresContainerParameters containerParameters, IContainerApi containerApi, ImageApi imageApi, ILogger? logger)
            : base(containerParameters, containerApi, imageApi, logger) =>
            _parameters = containerParameters;

        public string GetConnectionString() =>
            $"Host={(IsDockerInDocker ? IPAddress : IP.Loopback.ToString())};Port={(IsDockerInDocker ? 5432 : Ports![5432])};Username={_parameters.UserName};Password={_parameters.Password}";
    }
}
