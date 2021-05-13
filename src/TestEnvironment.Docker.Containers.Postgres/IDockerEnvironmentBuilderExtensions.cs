using System;
using System.Collections.Generic;
using Docker.DotNet;
using Microsoft.Extensions.Logging;

namespace TestEnvironment.Docker.Containers.Postgres
{
    public static class IDockerEnvironmentBuilderExtensions
    {
        public static PostgresContainerParameters DefaultParameters => new("postgres", "root", "securepass")
        {
            ImageName = "postgres",
            ContainerCleaner = new PostgresContainerCleaner(),
            ContainerWaiter = new PostgresContainerWaiter()
        };

        public static IDockerEnvironmentBuilder AddPostgresContainer(
            this IDockerEnvironmentBuilder builder,
            Func<PostgresContainerParameters, PostgresContainerParameters> paramsBuilder)
        {
            var parameters = paramsBuilder(builder.GetDefaultParameters());
            builder.AddContainer(parameters, (p, d, l) => new PostgresContainer(FixEnvironmentVariables(p), d, l));

            return builder;
        }

        public static IDockerEnvironmentBuilder AddPostgresContainer(
            this IDockerEnvironmentBuilder builder,
            Func<PostgresContainerParameters, IDockerClient, ILogger?, PostgresContainerParameters> paramsBuilder)
        {
            var parameters = paramsBuilder(builder.GetDefaultParameters(), builder.DockerClient, builder.Logger);
            builder.AddContainer(parameters, (p, d, l) => new PostgresContainer(FixEnvironmentVariables(p), d, l));

            return builder;
        }

        [Obsolete("This method is depricated and will be removed in upcoming versions.")]
        public static IDockerEnvironmentBuilder AddPostgresContainer(
            this IDockerEnvironmentBuilder builder,
            string name,
            string userName = "root",
            string password = "securepass",
            string imageName = "postgres",
            string tag = "latest",
            IDictionary<string, string>? environmentVariables = null,
            IDictionary<ushort, ushort>? ports = null,
            bool reuseContainer = false)
        {
            builder.AddPostgresContainer(p => p with
            {
                Name = name,
                UserName = userName,
                Password = password,
                ImageName = imageName,
                Tag = tag,
                EnvironmentVariables = environmentVariables,
                Ports = ports,
                Reusable = reuseContainer
            });

            return builder;
        }

        private static PostgresContainerParameters FixEnvironmentVariables(PostgresContainerParameters p) =>
            p with
            {
                EnvironmentVariables = new Dictionary<string, string>
                {
                    ["POSTGRES_USER"] = p.UserName,
                    ["POSTGRES_PASSWORD"] = p.Password
                }.MergeDictionaries(p.EnvironmentVariables),
            };

        private static PostgresContainerParameters GetDefaultParameters(this IDockerEnvironmentBuilder builder) =>
            builder.Logger switch
            {
                { } => DefaultParameters with
                {
                    ContainerWaiter = new PostgresContainerWaiter(builder.Logger),
                    ContainerCleaner = new PostgresContainerCleaner(builder.Logger)
                },
                null => DefaultParameters
            };
    }
}
