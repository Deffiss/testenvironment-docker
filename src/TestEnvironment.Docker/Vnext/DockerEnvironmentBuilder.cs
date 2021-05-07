using System;
using System.Collections.Generic;
using System.Linq;
using Docker.DotNet;
using Microsoft.Extensions.Logging;
using TestEnvironment.Docker.Vnext.ContainerOperations;
using TestEnvironment.Docker.Vnext.ImageOperations;
using static TestEnvironment.Docker.Vnext.DockerClientExtentions;
using static TestEnvironment.Docker.Vnext.StringExtensions;

namespace TestEnvironment.Docker.Vnext
{
    public class DockerEnvironmentBuilder : IDockerEnvironmentBuilder
    {
        private readonly Dictionary<ContainerParameters, Func<Container>> _containerFactories = new ();
        private IDictionary<string, string> _environmentVariables = new Dictionary<string, string>();
        private bool _isDockerInDocker = false;
        private string _environmentName = Guid.NewGuid().ToString().Substring(0, 10);

        public IDockerClient DockerClient { get; init; }

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

        public DockerEnvironmentBuilder(IDockerClient dockerClient, ILogger? logger) =>
            (DockerClient, Logger) = (dockerClient, logger);

        public IDockerEnvironmentBuilder DockerInDocker(bool isDockerInDocker = true)
        {
            _isDockerInDocker = isDockerInDocker;
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

            return new DockerEnvironment(_environmentName, containers, DockerClient, Logger);
        }
    }
}
