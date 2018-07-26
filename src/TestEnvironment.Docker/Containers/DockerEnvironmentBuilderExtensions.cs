namespace TestEnvironment.Docker.Containers
{
    public static class DockerEnvironmentBuilderExtensions
    {
        public static IDockerEnvironmentBuilder AddMssqlContainer(this IDockerEnvironmentBuilder builder, string name, string saPassword) =>
            builder.AddDependency(new MssqlContainer(builder.DockerClient, name, saPassword));

        public static IDockerEnvironmentBuilder AddElasticsearchContainer(this IDockerEnvironmentBuilder builder, string name) =>
            builder.AddDependency(new ElasticsearchContainer(builder.DockerClient, name));
    }
}
