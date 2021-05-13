using System;
using System.Collections.Generic;
using Docker.DotNet;
using Microsoft.Extensions.Logging;

namespace TestEnvironment.Docker.Containers.MariaDB
{
    public static class IDockerEnvironmentBuilderExtensions
    {
        public static MariaDBContainerParameters DefaultParameters => new ("maria", "password")
        {
            ImageName = "mariadb",
            ContainerCleaner = new MariaDBContainerCleaner(),
            ContainerWaiter = new MariaDBContainerWaiter()
        };

        public static IDockerEnvironmentBuilder AddMariaDBContainer(
            this IDockerEnvironmentBuilder builder,
            Func<MariaDBContainerParameters, MariaDBContainerParameters> paramsBuilder)
        {
            var parameters = paramsBuilder(builder.GetDefaultParameters());
            builder.AddContainer(parameters, (p, d, l) => new MariaDBContainer(FixEnvironmentVariables(p), d, l));

            return builder;
        }

        public static IDockerEnvironmentBuilder AddMariaDBContainer(
            this IDockerEnvironmentBuilder builder,
            Func<MariaDBContainerParameters, IDockerClient, ILogger?, MariaDBContainerParameters> paramsBuilder)
        {
            var parameters = paramsBuilder(builder.GetDefaultParameters(), builder.DockerClient, builder.Logger);
            builder.AddContainer(parameters, (p, d, l) => new MariaDBContainer(FixEnvironmentVariables(p), d, l));

            return builder;
        }

        [Obsolete("This method is depricated and will be removed in upcoming versions.")]
        public static IDockerEnvironmentBuilder AddMariaDBContainer(this IDockerEnvironmentBuilder builder, string name, string rootPassword, string imageName = "mariadb", string tag = "latest", IDictionary<string, string>? environmentVariables = null, IDictionary<ushort, ushort>? ports = null, bool reuseContainer = false)
        {
            builder.AddMariaDBContainer(p => p with
            {
                Name = name,
                RootPassword = rootPassword,
                ImageName = imageName,
                Tag = tag,
                EnvironmentVariables = environmentVariables,
                Ports = ports,
                Reusable = reuseContainer
            });

            return builder;
        }

        private static MariaDBContainerParameters FixEnvironmentVariables(MariaDBContainerParameters p) =>
            p with
            {
                EnvironmentVariables = new Dictionary<string, string>
                {
                    ["MYSQL_ROOT_PASSWORD"] = p.RootPassword
                }.MergeDictionaries(p.EnvironmentVariables),
            };

        private static MariaDBContainerParameters GetDefaultParameters(this IDockerEnvironmentBuilder builder) =>
            builder.Logger switch
            {
                { } => DefaultParameters with
                {
                    ContainerWaiter = new MariaDBContainerWaiter(builder.Logger),
                    ContainerCleaner = new MariaDBContainerCleaner(builder.Logger)
                },
                null => DefaultParameters
            };
    }
}
