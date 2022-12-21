using Docker.DotNet;
using Microsoft.Extensions.Logging;
using TestEnvironment.Docker.ContainerOperations;
using TestEnvironment.Docker.ImageOperations;
using IP = System.Net.IPAddress;

namespace TestEnvironment.Docker.Containers.RabbitMQ
{
    public class RabbitMQContainer : Container
    {
        private readonly RabbitMQContainerParameters _parameters;

        public string UserName => _parameters.UserName;

        public string Password => _parameters.Password;

        public int Port => IsDockerInDocker ? 5672 : Ports![5672];

        public string? Host => IsDockerInDocker ? IPAddress : IP.Loopback.ToString();

#pragma warning disable SA1201 // Elements should appear in the correct order
        public RabbitMQContainer(RabbitMQContainerParameters containerParameters)
#pragma warning restore SA1201 // Elements should appear in the correct order
            : base(containerParameters) =>
            _parameters = containerParameters;

        public RabbitMQContainer(RabbitMQContainerParameters containerParameters, IDockerClient dockerClient)
            : base(containerParameters, dockerClient) =>
            _parameters = containerParameters;

        public RabbitMQContainer(RabbitMQContainerParameters containerParameters, IDockerClient dockerClient, ILogger? logger)
            : base(containerParameters, dockerClient, logger) =>
            _parameters = containerParameters;

        public RabbitMQContainer(RabbitMQContainerParameters containerParameters, IContainerApi containerApi, ImageApi imageApi, ILogger? logger)
            : base(containerParameters, containerApi, imageApi, logger) =>
            _parameters = containerParameters;
    }
}