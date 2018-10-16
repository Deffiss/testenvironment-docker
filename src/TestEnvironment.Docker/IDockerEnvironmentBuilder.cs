using Docker.DotNet;
using Microsoft.Extensions.Logging;
using System.IO;

namespace TestEnvironment.Docker
{
    public interface IDockerEnvironmentBuilder
    {
        DockerClient DockerClient { get; }

        ILogger Logger { get; }

        bool IsDockerInDocker { get; }

        IDockerEnvironmentBuilder DockerInDocker();

        IDockerEnvironmentBuilder UseDefaultNetwork();

        IDockerEnvironmentBuilder WithLogger(ILogger logger);

        IDockerEnvironmentBuilder SetName(string environmentName);

        IDockerEnvironmentBuilder SetVariable(params (string Name, string Value)[] variables);

        IDockerEnvironmentBuilder AddDependency(IDependency dependency);

        IDockerEnvironmentBuilder AddContainer(string name, string imageName, string tag = "latest", (string Name, string Value)[] environmentVariables = null);

        IDockerEnvironmentBuilder AddFromCompose(Stream composeFileStream);

        IDockerEnvironmentBuilder AddFromDockerfile(Stream dockerfileStream);

        DockerEnvironment Build();
    }
}
