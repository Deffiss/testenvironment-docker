using System.Collections.Generic;

namespace TestEnvironment.Docker.Containers.Firebird
{
    public static class DockerEnvironmentBuilderExtensions
    {
        public static IDockerEnvironmentBuilder AddFirebirdContainer(
            this IDockerEnvironmentBuilder builder,
            string name,
            string userName = "user1",
            string password = "p4ssw0rd",
            string imageName = "jacobalberty/firebird",
            string tag = "latest",
            IDictionary<string, string> environmentVariables = null,
            IDictionary<ushort, ushort> ports = null,
            bool reuseContainer = false)
        {
            return builder.AddDependency(
                new FirebirdContainer(
                    builder.DockerClient,
                    name.GetContainerName(builder.EnvironmentName),
                    userName,
                    password,
                    imageName,
                    tag,
                    new Dictionary<string, string>
                    {
                        ["FIREBIRD_USER"] = userName,
                        ["FIREBIRD_PASSWORD"] = password,
                        ["FIREBIRD_DATABASE"] = "SampleDatabase"

                        // ,
                        // ["EnableWireCrypt"] = "true",
                        // ["ISC_PASSWORD"] = password,
                    }.MergeDictionaries(environmentVariables),
                    ports,
                    builder.IsDockerInDocker,
                    reuseContainer,
                    new FirebirdContainerWaiter(builder.Logger),
                    new FirebirdContainerCleaner(builder.Logger, userName),
                    builder.Logger));
        }
    }
}
