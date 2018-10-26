namespace TestEnvironment.Docker.Containers
{
    public static class DockerEnvironmentBuilderExtensions
    {
        public static IDockerEnvironmentBuilder AddMssqlContainer(this IDockerEnvironmentBuilder builder, string name, string saPassword, string imageName = "microsoft/mssql-server-linux", string tag = "latest") =>
            builder.AddDependency(new MssqlContainer(builder.DockerClient, name.GetContainerName(builder.EnvitronmentName), saPassword, imageName, tag, builder.Logger, builder.IsDockerInDocker));

        public static IDockerEnvironmentBuilder AddElasticsearchContainer(this IDockerEnvironmentBuilder builder, string name, string imageName = "docker.elastic.co/elasticsearch/elasticsearch-oss", string tag = "6.2.4") =>
            builder.AddDependency(new ElasticsearchContainer(builder.DockerClient, name.GetContainerName(builder.EnvitronmentName), imageName, tag, builder.Logger, builder.IsDockerInDocker));
    }
}
