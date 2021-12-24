using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Docker.DotNet;
using Microsoft.Extensions.Logging;
using TestEnvironment.Docker.ContainerOperations;
using TestEnvironment.Docker.DockerOperations;
using TestEnvironment.Docker.ImageOperations;
using static TestEnvironment.Docker.DockerClientExtentions;
using static TestEnvironment.Docker.StringExtensions;

namespace TestEnvironment.Docker
{
    public class DockerEnvironmentBuilder : IDockerEnvironmentBuilder
    {
        private readonly Dictionary<ContainerParameters, Func<Container>> _containerFactories = new();
        private IDictionary<string, string> _environmentVariables = new Dictionary<string, string>();
        private bool _isWsl2 = false;
        private bool _isDockerInDocker = false;
        private string _environmentName = Guid.NewGuid().ToString().Substring(0, 10);

        public IDockerClient DockerClient { get; private set; }

        public ILogger? Logger { get; init; }

#pragma warning disable SA1201 // Elements should appear in the correct order
        public DockerEnvironmentBuilder()
#pragma warning restore SA1201 // Elements should appear in the correct order
            : this(CreateDefaultDockerClient(), null)
        {
        }

        public DockerEnvironmentBuilder(IDockerClient dockerClient)
            : this(dockerClient, null)
        {
        }

        public DockerEnvironmentBuilder(ILogger logger)
            : this(CreateDefaultDockerClient(), logger)
        {
        }

        public DockerEnvironmentBuilder(IDockerClient dockerClient, ILogger? logger) =>
            (DockerClient, Logger) = (dockerClient, logger ?? Logger);

        public IDockerEnvironmentBuilder DockerInDocker()
        {
            _isDockerInDocker = true;
            return this;
        }

        public IDockerEnvironmentBuilder UseWsl2(int port = 2375)
        {
            _isWsl2 = true;

            // Use loopback address to connect to Docker daemon in WSL2.
            DockerClient = CreateWSL2DockerClient();
            return this;
        }

        public IDockerEnvironmentBuilder SetName(string environmentName)
        {
            _environmentName = environmentName;
            return this;
        }

        public IDockerEnvironmentBuilder SetEnvironmentVariables(IDictionary<string, string> environmentVariables)
        {
            _environmentVariables = environmentVariables;
            return this;
        }

        public IDockerEnvironmentBuilder AddContainer(Func<ContainerParameters, ContainerParameters> paramsBuilder)
        {
            var parameters = paramsBuilder(new ContainerParameters("hello", "docker/getting-started"));
            AddContainer(parameters, (p, d, l) => new Container(p, d, l));
            return this;
        }

        public IDockerEnvironmentBuilder AddContainer(Func<ContainerParameters, IDockerClient, ILogger?, ContainerParameters> paramsBuilder)
        {
            var parameters = paramsBuilder(new ContainerParameters("hello", "docker/getting-started"), DockerClient, Logger);
            AddContainer(parameters, (p, d, l) => new Container(p, d, l));
            return this;
        }

        public IDockerEnvironmentBuilder AddContainer<TParams>(TParams containerParameters, Func<TParams, IDockerClient, ILogger?, Container> containerFactory)
            where TParams : ContainerParameters
        {
            _containerFactories.Add(containerParameters, () =>
            {
                var envParameters = containerParameters with
                {
                    Name = GetContainerName(_environmentName, containerParameters.Name),
                    EnvironmentVariables = _environmentVariables.MergeDictionaries(containerParameters.EnvironmentVariables),
                    IsDockerInDocker = _isDockerInDocker,
                };

                return containerFactory(envParameters, DockerClient, Logger);
            });
            return this;
        }

        public IDockerEnvironment Build()
        {
            var containers = _containerFactories.Values.Select(cf => cf()).ToArray();

            return _isWsl2
                ? new DockerEnvironment(_environmentName, containers, DockerClient, new DockerInWs2Initializer(DockerClient, Logger), Logger)
                : new DockerEnvironment(_environmentName, containers, DockerClient, Logger);
        }
    }
}
