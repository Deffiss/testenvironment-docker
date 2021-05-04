using System;
using System.Collections.Generic;
using Docker.DotNet;
using Microsoft.Extensions.Logging;
using TestEnvironment.Docker.Vnext.ContainerOperations;
using TestEnvironment.Docker.Vnext.ImageOperations;
using static TestEnvironment.Docker.Vnext.DockerClientExtentions;

namespace TestEnvironment.Docker.Vnext
{
    public class DockerEnvironmentBuilder : IDockerEnvironmentBuilder
    {
        private readonly IDockerClient _dockerClient;
        private readonly Dictionary<ContainerParameters, Func<Container>> _containers = new ();
        private IDictionary<string, string> _environmentVariables = new Dictionary<string, string>();
        private bool _isDockerInDocker = false;
        private string _environmentName = Guid.NewGuid().ToString().Substring(0, 10);
        private ILogger? _logger;

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
            (_dockerClient, _logger) = (dockerClient, logger);

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

        public IDockerEnvironmentBuilder AddContainer(Func<ContainerParameters, ContainerParameters> paramsBuilder)
        {
            var parameters = paramsBuilder(new ContainerParameters("hello", "docker/getting-started"));
            AddContainer(parameters, (p, d, l) => new Container(p, d, l));
            return this;
        }

        public IDockerEnvironmentBuilder AddContainer(Func<ContainerParameters, IDockerClient, ILogger?, ContainerParameters> paramsBuilder)
        {
            var parameters = paramsBuilder(new ContainerParameters("hello", "docker/getting-started"), _dockerClient, _logger);
            AddContainer(parameters, (p, d, l) => new Container(p, d, l));
            return this;
        }

        public IDockerEnvironmentBuilder SetEnvironmentVariables(IDictionary<string, string> environmentVariables)
        {
            _environmentVariables = environmentVariables;
            return this;
        }

        public IDockerEnvironmentBuilder AddContainer<TParams>(TParams containerParameters, Func<TParams, IDockerClient, ILogger?, Container> containerFactory)
            where TParams : ContainerParameters
        {
            _containers.Add(containerParameters, () =>
            {
                var envParameters = containerParameters with
                {
                    Name = $"{_environmentName}-{containerParameters.Name}",
                    EnvironmentVariables = _environmentVariables.MergeDictionaries(containerParameters.EnvironmentVariables),
                    IsDockerInDocker = _isDockerInDocker,
                };

                return containerFactory(envParameters, _dockerClient, _logger);
            });
            return this;
        }

        public IDockerEnvironment Build()
        {
            throw new NotImplementedException();
        }
    }
}
