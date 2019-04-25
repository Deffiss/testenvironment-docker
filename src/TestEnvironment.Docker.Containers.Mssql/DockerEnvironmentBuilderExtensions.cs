using System.Collections.Generic;

namespace TestEnvironment.Docker.Containers.Mssql
{
    public static class DockerEnvironmentBuilderExtensions
    {
        public static IDockerEnvironmentBuilder AddMssqlContainer(this IDockerEnvironmentBuilder builder, string name, string saPassword, string imageName = "microsoft/mssql-server-linux", string tag = "latest", IDictionary<string, string> environmentVariables = null, bool reuseContainer = false) =>
            builder.AddDependency(new MssqlContainer(builder.DockerClient, name.GetContainerName(builder.EnvironmentName), saPassword, imageName, tag, environmentVariables, builder.IsDockerInDocker, reuseContainer, builder.Logger));
    }
}
