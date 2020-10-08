using System.Collections.Generic;

namespace TestEnvironment.Docker.Containers.Oracle
{
    public static class DockerEnvironmentBuilderExtensions
    {
        public static IDockerEnvironmentBuilder AddOracleContainer(
            this IDockerEnvironmentBuilder builder,
            string name,
            string userName = "system",
            string password = "oracle",
            string imageName = "wnameless/oracle-xe-11g-r2",
            string tag = "latest",
            IDictionary<string, string> environmentVariables = null,
            IDictionary<ushort, ushort> ports = null,
            bool reuseContainer = false)
        {
            return builder.AddDependency(
                new OracleContainer(
                    builder.DockerClient,
                    name.GetContainerName(builder.EnvironmentName),
                    userName,
                    password,
                    imageName,
                    tag,
                    new Dictionary<string, string>
                    {
                    }.MergeDictionaries(environmentVariables),
                    ports,
                    builder.IsDockerInDocker,
                    reuseContainer,
                    new OracleContainerWaiter(builder.Logger),
                    new OracleContainerCleaner(builder.Logger, userName),
                    builder.Logger));
        }
    }
}
