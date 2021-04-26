using System;
using System.Collections.Generic;
using Docker.DotNet;
using Microsoft.Extensions.Logging;
using TestEnvironment.Docker.Vnext.ContainerOperations;
using TestEnvironment.Docker.Vnext.ImageOperations;
using static TestEnvironment.Docker.Vnext.DockerClientExtentions;

namespace TestEnvironment.Docker.Vnext
{
    public class DockerEnvironmentBuilder
    {
        private readonly List<Container> _containers = new List<Container>();
        private IDictionary<string, string> _environmentVariables = new Dictionary<string, string>();

        public IImageApi ImageApi { get; init; }

        public IContainerApi ContainerApi { get; init; }

        public ILogger? Logger { get; init; } = LoggerFactory.Create(lb => lb.AddConsole().AddDebug()).CreateLogger<DockerEnvironment>();

        public bool IsDockerInDocker { get; private set; } = false;

        public bool DefaultNetwork { get; private set; } = false;

        public string EnvironmentName { get; private set; } = Guid.NewGuid().ToString().Substring(0, 10);

#pragma warning disable SA1201 // Elements should appear in the correct order
        public DockerEnvironmentBuilder()
#pragma warning restore SA1201 // Elements should appear in the correct order
            : this(CreateDefaultDockerClient())
        {
        }

        public DockerEnvironmentBuilder(ILogger logger)
            : this(new ImageApi(logger), new ContainerApi(logger), logger)
        {
        }

        public DockerEnvironmentBuilder(IDockerClient dockerClient)
            : this(new ImageApi(dockerClient), new ContainerApi(dockerClient), null)
        {
        }

        public DockerEnvironmentBuilder(IImageApi imageApi, IContainerApi containerApi, ILogger? logger) =>
            (ImageApi, ContainerApi, Logger) = (imageApi, containerApi, logger);
    }
}
