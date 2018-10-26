using Docker.DotNet;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace TestEnvironment.Docker
{
    public interface IDockerEnvironmentBuilder
    {
        DockerClient DockerClient { get; }

        ILogger Logger { get; }

        bool IsDockerInDocker { get; }

        string EnvitronmentName { get; }

        IDockerEnvironmentBuilder DockerInDocker();

        IDockerEnvironmentBuilder UseDefaultNetwork();

        IDockerEnvironmentBuilder WithLogger(ILogger logger);

        IDockerEnvironmentBuilder SetName(string environmentName);

        IDockerEnvironmentBuilder SetVariable(IDictionary<string, string> variables);

        IDockerEnvironmentBuilder AddDependency(IDependency dependency);

        IDockerEnvironmentBuilder AddContainer(string name, string imageName, string tag = "latest", IDictionary<string, string> environmentVariables = null, Func<Container, Task<bool>> waitFunc = null);

        IDockerEnvironmentBuilder AddFromCompose(Stream composeFileStream);

        IDockerEnvironmentBuilder AddFromDockerfile(Stream dockerfileStream);

        DockerEnvironment Build();
    }
}
