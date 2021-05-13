using System;
using System.Collections.Generic;
using Docker.DotNet;
using Microsoft.Extensions.Logging;

namespace TestEnvironment.Docker.Containers.Mssql
{
    public static class IDockerEnvironmentBuilderExtensions
    {
        public static MssqlContainerParameters DefaultParameters => new ("mssql", "password")
        {
            ImageName = "mariadb",
            ContainerCleaner = new MssqlContainerCleaner(),
            ContainerWaiter = new MssqlContainerWaiter()
        };

        public static IDockerEnvironmentBuilder AddMssqlContainer(
            this IDockerEnvironmentBuilder builder,
            Func<MssqlContainerParameters, MssqlContainerParameters> paramsBuilder)
        {
            var parameters = paramsBuilder(builder.GetDefaultParameters());
            builder.AddContainer(parameters, (p, d, l) => new MssqlContainer(FixEnvironmentVariables(p), d, l));

            return builder;
        }

        public static IDockerEnvironmentBuilder AddMssqlContainer(
            this IDockerEnvironmentBuilder builder,
            Func<MssqlContainerParameters, IDockerClient, ILogger?, MssqlContainerParameters> paramsBuilder)
        {
            var parameters = paramsBuilder(builder.GetDefaultParameters(), builder.DockerClient, builder.Logger);
            builder.AddContainer(parameters, (p, d, l) => new MssqlContainer(FixEnvironmentVariables(p), d, l));

            return builder;
        }

        [Obsolete("This method is depricated and will be removed in upcoming versions.")]
        public static IDockerEnvironmentBuilder AddMssqlContainer(
            this IDockerEnvironmentBuilder builder,
            string name,
            string saPassword,
            string imageName = "mcr.microsoft.com/mssql/server",
            string tag = "latest",
            IDictionary<string, string>? environmentVariables = null,
            IDictionary<ushort, ushort>? ports = null,
            bool reuseContainer = false)
        {
            builder.AddMssqlContainer(p => p with
            {
                Name = name,
                SAPassword = saPassword,
                ImageName = imageName,
                Tag = tag,
                EnvironmentVariables = environmentVariables,
                Ports = ports,
                Reusable = reuseContainer
            });

            return builder;
        }

        private static MssqlContainerParameters FixEnvironmentVariables(MssqlContainerParameters p) =>
            p with
            {
                EnvironmentVariables = new Dictionary<string, string>
                {
                    ["ACCEPT_EULA"] = "Y",
                    ["SA_PASSWORD"] = p.SAPassword,
                    ["MSSQL_PID"] = "Express"
                }.MergeDictionaries(p.EnvironmentVariables),
            };

        private static MssqlContainerParameters GetDefaultParameters(this IDockerEnvironmentBuilder builder) =>
            builder.Logger switch
            {
                { } => DefaultParameters with
                {
                    ContainerWaiter = new MssqlContainerWaiter(builder.Logger),
                    ContainerCleaner = new MssqlContainerCleaner(builder.Logger)
                },
                null => DefaultParameters
            };
    }
}
