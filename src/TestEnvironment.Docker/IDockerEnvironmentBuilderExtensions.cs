using System;
using System.Collections.Generic;
using Docker.DotNet;
using Microsoft.Extensions.Logging;
using TestEnvironment.Docker.ContainerLifecycle;

namespace TestEnvironment.Docker
{
    public static class IDockerEnvironmentBuilderExtensions
    {
        public static IDockerEnvironmentBuilder AddContainerFromDockerfile(
            this IDockerEnvironmentBuilder builder,
            Func<ContainerFromDockerfileParameters, ContainerFromDockerfileParameters> paramsBuilder)
        {
            var parameters = paramsBuilder(new ContainerFromDockerfileParameters("hello", "Dockerfile"));
            builder.AddContainer(parameters, (p, d, l) => new ContainerFromDockerfile(p, d, l));

            return builder;
        }

        public static IDockerEnvironmentBuilder AddContainerFromDockerfile(
            this IDockerEnvironmentBuilder builder,
            Func<ContainerFromDockerfileParameters, IDockerClient, ILogger?, ContainerFromDockerfileParameters> paramsBuilder)
        {
            var parameters = paramsBuilder(new ContainerFromDockerfileParameters("hello", "Dockerfile"), builder.DockerClient, builder.Logger);
            builder.AddContainer(parameters, (p, d, l) => new ContainerFromDockerfile(p, d, l));

            return builder;
        }

        [Obsolete("This method is depricated and will be removed in upcoming versions.")]
        public static IDockerEnvironmentBuilder AddFromDockerfile(
            this IDockerEnvironmentBuilder builder,
            string name,
            string dockerfile,
            IDictionary<string, string>? buildArgs = null,
            string context = ".",
            IDictionary<string, string>? environmentVariables = null,
            IDictionary<ushort, ushort>? ports = null,
            bool reuseContainer = false,
            IContainerWaiter? containerWaiter = null,
            IContainerCleaner? containerCleaner = null)
        {
            builder.AddContainerFromDockerfile(p => p with
            {
                Name = name,
                Dockerfile = dockerfile,
                BuildArgs = buildArgs,
                Context = context,
                EnvironmentVariables = environmentVariables,
                Ports = ports,
                Reusable = reuseContainer,
                ContainerWaiter = containerWaiter,
                ContainerCleaner = containerCleaner
            });

            return builder;
        }
    }
}
