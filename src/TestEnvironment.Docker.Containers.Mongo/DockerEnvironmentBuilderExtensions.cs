using System.Collections.Generic;

namespace TestEnvironment.Docker.Containers.Mongo
{
    public static class DockerEnvironmentBuilderExtensions
    {
        public static IDockerEnvironmentBuilder AddMongoContainer(
            this IDockerEnvironmentBuilder builder,
            string name,
            string userName = "root",
            string userPassword = "example",
            string imageName = "mongo",
            string tag = "latest",
            IDictionary<string, string> environmentVariables = null,
            bool reuseContainer = false)
        {
            return builder.AddDependency(
                new MongoContainer(
                    builder.DockerClient,
                    name.GetContainerName(builder.EnvironmentName),
                    userName,
                    userPassword,
                    imageName,
                    tag,
                    new Dictionary<string, string>
                    {
                        { "MONGO_INITDB_ROOT_USERNAME", userName },
                        { "MONGO_INITDB_ROOT_PASSWORD", userPassword }
                    }.MergeDictionaries(environmentVariables),
                    builder.IsDockerInDocker,
                    reuseContainer,
                    new MongoContainerWaiter(builder.Logger),
                    new MongoContainerCleaner(builder.Logger),
                    builder.Logger));
        }
    }
}