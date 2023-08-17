using System;
using System.Collections.Generic;
using Docker.DotNet;
using Microsoft.Extensions.Logging;
using TestEnvironment.Docker.ContainerOperations;
using TestEnvironment.Docker.ImageOperations;

namespace TestEnvironment.Docker
{
    public interface IDockerEnvironmentBuilder
    {
        IDockerClient DockerClient { get; }

        ILogger? Logger { get; }

        IDockerEnvironmentBuilder DockerInDocker();

        IDockerEnvironmentBuilder SetName(string environmentName);

        IDockerEnvironmentBuilder UseWsl2(int port = 2375);

        IDockerEnvironmentBuilder SetEnvironmentVariables(IDictionary<string, string> environmentVariables);

        IDockerEnvironmentBuilder AddContainer(Func<ContainerParameters, ContainerParameters> paramsBuilder);

        IDockerEnvironmentBuilder AddContainer<TParams>(TParams containerParameters, Func<TParams, IDockerClient, ILogger?, Container> containerFactory)
            where TParams : ContainerParameters;

        IDockerEnvironmentBuilder WithContainerApi(Func<IDockerClient, ILogger?, IContainerApi> containerApiFactory);

        IDockerEnvironmentBuilder WithImageApi(Func<IDockerClient, ILogger?, IImageApi> imageApiFactory);

        IDockerEnvironment Build();
    }
}
