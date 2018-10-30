using System.Collections.Generic;

namespace TestEnvironment.Docker.Containers
{
    public static class DockerEnvironmentBuilderExtensions
    {
        public static IDockerEnvironmentBuilder AddMssqlContainer(this IDockerEnvironmentBuilder builder, string name, string saPassword, string imageName = "microsoft/mssql-server-linux", string tag = "latest", IDictionary<string, string> environmentVariables = null, bool reuseContainer = false) =>
            builder.AddDependency(new MssqlContainer(builder.DockerClient, name.GetContainerName(builder.EnvitronmentName), saPassword, imageName, tag, environmentVariables, builder.IsDockerInDocker, reuseContainer, builder.Logger));

        public static IDockerEnvironmentBuilder AddElasticsearchContainer(this IDockerEnvironmentBuilder builder, string name, string imageName = "docker.elastic.co/elasticsearch/elasticsearch-oss", string tag = "6.2.4", IDictionary<string, string> environmentVariables = null, bool reuseContainer = false) =>
            builder.AddDependency(new ElasticsearchContainer(builder.DockerClient, name.GetContainerName(builder.EnvitronmentName), imageName, tag, environmentVariables, builder.IsDockerInDocker, reuseContainer, builder.Logger));
    }
}
