using System;
using Docker.DotNet;
using Microsoft.Extensions.Logging;

namespace TestEnvironment.Docker.Vnext
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
            var parameters = paramsBuilder(new ContainerFromDockerfileParameters("hello", "Dockerfile"), );
            builder.AddContainer()
            builder.AddContainer(parameters, (p, d, l) => new ContainerFromDockerfile(p, d, l));

            return builder;
        }
    }
}
