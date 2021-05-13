using System;
using System.Collections.Generic;
using Docker.DotNet;
using Microsoft.Extensions.Logging;
using IP = System.Net.IPAddress;

namespace TestEnvironment.Docker.Containers.Mail
{
    public static class IDockerEnvironmentBuilderExtensions
    {
        public static MailContainerParameters DefaultParameters => new("mail")
        {
            ImageName = "mailhog/mailhog",
            ContainerCleaner = new MailContainerCleaner(),
            ContainerWaiter = new MailContainerWaiter()
        };

        public static IDockerEnvironmentBuilder AddMailContainer(
            this IDockerEnvironmentBuilder builder,
            Func<MailContainerParameters, MailContainerParameters> paramsBuilder)
        {
            var parameters = paramsBuilder(builder.GetDefaultParameters());
            builder.AddContainer(parameters, (p, d, l) => new MailContainer(FixEnvironmentVariables(p), d, l));

            return builder;
        }

        public static IDockerEnvironmentBuilder AddMailContainer(
            this IDockerEnvironmentBuilder builder,
            Func<MailContainerParameters, IDockerClient, ILogger?, MailContainerParameters> paramsBuilder)
        {
            var parameters = paramsBuilder(builder.GetDefaultParameters(), builder.DockerClient, builder.Logger);
            builder.AddContainer(parameters, (p, d, l) => new MailContainer(FixEnvironmentVariables(p), d, l));

            return builder;
        }

        [Obsolete("This method is depricated and will be removed in upcoming versions.")]
        public static IDockerEnvironmentBuilder AddMailContainer(
            this IDockerEnvironmentBuilder builder,
            string name,
            string imageName = "mailhog/mailhog",
            string tag = "latest",
            ushort smptPort = 1025,
            ushort apiPort = 8025,
            string deleteEndpoint = "api/v1/messages",
            IDictionary<string, string>? environmentVariables = null,
            IDictionary<ushort, ushort>? ports = null,
            bool reuseContainer = false)
        {
            builder.AddMailContainer(p => p with
            {
                Name = name,
                ImageName = imageName,
                Tag = tag,
                SmptPort = smptPort,
                ApiPort = apiPort,
                DeleteEndpoint = deleteEndpoint,
                EnvironmentVariables = environmentVariables,
                Ports = ports,
                Reusable = reuseContainer
            });

            return builder;
        }

        private static MailContainerParameters FixEnvironmentVariables(MailContainerParameters p) =>
            p with
            {
                EnvironmentVariables = new Dictionary<string, string>
                {
                    // TODO: allign env vars
                }.MergeDictionaries(p.EnvironmentVariables),
            };

        private static MailContainerParameters GetDefaultParameters(this IDockerEnvironmentBuilder builder) =>
            builder.Logger switch
            {
                { } => DefaultParameters with
                {
                    ContainerWaiter = new MailContainerWaiter(builder.Logger),
                    ContainerCleaner = new MailContainerCleaner(builder.Logger)
                },
                null => DefaultParameters
            };
    }
}
