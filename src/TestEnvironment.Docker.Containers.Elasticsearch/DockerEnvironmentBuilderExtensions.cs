using System.Collections.Generic;

namespace TestEnvironment.Docker.Containers.Elasticsearch
{
    public static class DockerEnvironmentBuilderExtensions
    {
        public static IDockerEnvironmentBuilder AddElasticsearchContainer(this IDockerEnvironmentBuilder builder, string name, string imageName = "docker.elastic.co/elasticsearch/elasticsearch-oss", string tag = "6.2.4", IDictionary<string, string> environmentVariables = null, bool reuseContainer = false) =>
            builder.AddDependency(new ElasticsearchContainer(builder.DockerClient, name.GetContainerName(builder.EnvironmentName), imageName, tag, environmentVariables, builder.IsDockerInDocker, reuseContainer, builder.Logger));
    }
}
