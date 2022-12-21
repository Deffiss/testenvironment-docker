using System;
using System.Collections.Generic;
using Docker.DotNet;
using Microsoft.Extensions.Logging;

namespace TestEnvironment.Docker.Containers.AzureSqlEdge
{
    public static class IDockerEnvironmentBuilderExtensions
    {
        public static AzureSqlEdgeContainerParameters DefaultParameters => new("azuresqledge", "password")
        {
            ImageName = "mcr.microsoft.com/azure-sql-edge",
            ContainerCleaner = new AzureSqlEdgeContainerCleaner(),
            ContainerWaiter = new AzureSqlEdgeContainerWaiter()
        };

        public static IDockerEnvironmentBuilder AddAzureSqlEdgeContainer(
            this IDockerEnvironmentBuilder builder,
            Func<AzureSqlEdgeContainerParameters, AzureSqlEdgeContainerParameters> paramsBuilder)
        {
            var parameters = paramsBuilder(builder.GetDefaultParameters());
            builder.AddContainer(parameters, (p, d, l) => new AzureSqlEdgeContainer(FixEnvironmentVariables(p), d, l));

            return builder;
        }

        public static IDockerEnvironmentBuilder AddAzureSqlEdgeContainer(
            this IDockerEnvironmentBuilder builder,
            Func<AzureSqlEdgeContainerParameters, IDockerClient, ILogger?, AzureSqlEdgeContainerParameters> paramsBuilder)
        {
            var parameters = paramsBuilder(builder.GetDefaultParameters(), builder.DockerClient, builder.Logger);
            builder.AddContainer(parameters, (p, d, l) => new AzureSqlEdgeContainer(FixEnvironmentVariables(p), d, l));

            return builder;
        }

        [Obsolete("This method is deprecated and will be removed in upcoming versions.")]
        public static IDockerEnvironmentBuilder AddAzureSqlEdgeContainer(
            this IDockerEnvironmentBuilder builder,
            string name,
            string saPassword,
            string imageName = "mcr.microsoft.com/azure-sql-edge",
            string tag = "latest",
            IDictionary<string, string>? environmentVariables = null,
            IDictionary<ushort, ushort>? ports = null,
            bool reuseContainer = false)
        {
            AddAzureSqlEdgeContainer(
                builder,
                p => p with
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

        private static AzureSqlEdgeContainerParameters FixEnvironmentVariables(AzureSqlEdgeContainerParameters p) =>
            p with
            {
                EnvironmentVariables = new Dictionary<string, string>
                {
                    ["ACCEPT_EULA"] = "Y",
                    ["SA_PASSWORD"] = p.SAPassword,
                }.MergeDictionaries(p.EnvironmentVariables),
            };

        private static AzureSqlEdgeContainerParameters GetDefaultParameters(this IDockerEnvironmentBuilder builder) =>
            builder.Logger switch
            {
                { } => DefaultParameters with
                {
                    ContainerWaiter = new AzureSqlEdgeContainerWaiter(builder.Logger),
                    ContainerCleaner = new AzureSqlEdgeContainerCleaner(builder.Logger)
                },
                null => DefaultParameters
            };
    }
}