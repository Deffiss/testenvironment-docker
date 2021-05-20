using System;
using System.Collections.Generic;
using Docker.DotNet;
using Microsoft.Extensions.Logging;

namespace TestEnvironment.Docker
{
    public interface IDockerEnvironmentBuilder
    {
        IDockerClient DockerClient { get; }

        ILogger? Logger { get; }

        IDockerEnvironmentBuilder DockerInDocker(bool dockerInDocker = true);

        IDockerEnvironmentBuilder SetName(string environmentName);

        IDockerEnvironmentBuilder SetEnvironmentVariables(IDictionary<string, string> environmentVariables);

        IDockerEnvironmentBuilder AddContainer(Func<ContainerParameters, ContainerParameters> paramsBuilder);

        IDockerEnvironmentBuilder AddContainer<TParams>(TParams containerParameters, Func<TParams, IDockerClient, ILogger?, Container> containerFactory)
            where TParams : ContainerParameters;

        IDockerEnvironment Build();
    }
}
