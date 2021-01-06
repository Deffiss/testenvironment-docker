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
            string imageName = "davidgarciavivesdn/oracle-test",
            string tag = "latest",
            IDictionary<string, string> environmentVariables = null,
            IDictionary<ushort, ushort> ports = null,
            bool reuseContainer = false,
            IList<string> entrypoint = null)
        {
            return builder.AddDependency(
                new OracleContainer(
                    builder.DockerClient,
                    name.GetContainerName(builder.EnvironmentName),
                    userName,
                    password,
                    imageName,
                    tag,
                    environmentVariables,
                    ports,
                    builder.IsDockerInDocker,
                    reuseContainer,
                    builder.Logger,
                    entrypoint));
        }
    }
}
