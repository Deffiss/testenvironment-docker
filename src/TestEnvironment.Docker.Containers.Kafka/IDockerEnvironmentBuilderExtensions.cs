using System;
using System.Collections.Generic;
using Docker.DotNet;
using Microsoft.Extensions.Logging;

namespace TestEnvironment.Docker.Containers.Kafka
{
    public static class IDockerEnvironmentBuilderExtensions
    {
        public static ContainerParameters DefaultParameters => new("kafka", "johnnypark/kafka-zookeeper")
        {
            EnvironmentVariables = new Dictionary<string, string>
            {
                ["ADVERTISED_HOST"] = "localhost",
            },
            Ports = new Dictionary<ushort, ushort>
            {
                [9092] = 9092,
                [2181] = 2181,
            },
            Tag = "2.6.0",
            ContainerCleaner = new KafkaContainerCleaner(),
            ContainerWaiter = new KafkaContainerWaiter()
        };

        public static IDockerEnvironmentBuilder AddKafkaContainer(
            this IDockerEnvironmentBuilder builder,
            Func<ContainerParameters, ContainerParameters> paramsBuilder)
        {
            var parameters = paramsBuilder(builder.GetDefaultParameters());
            builder.AddContainer(parameters, (p, d, l) => new KafkaContainer(p, d, l));

            return builder;
        }

        public static IDockerEnvironmentBuilder AddKafkaContainer(
            this IDockerEnvironmentBuilder builder,
            Func<ContainerParameters, IDockerClient, ILogger?, ContainerParameters> paramsBuilder)
        {
            var parameters = paramsBuilder(builder.GetDefaultParameters(), builder.DockerClient, builder.Logger);
            builder.AddContainer(parameters, (p, d, l) => new KafkaContainer(p, d, l));

            return builder;
        }

        [Obsolete("This method is depricated and will be removed in upcoming versions.")]
        public static IDockerEnvironmentBuilder AddKafkaContainer(
            this IDockerEnvironmentBuilder builder,
            string name,
            string imageName = "johnnypark/kafka-zookeeper",
            string tag = "latest",
            IDictionary<string, string>? environmentVariables = null,
            IDictionary<ushort, ushort>? ports = null,
            bool reuseContainer = false)
        {
            builder.AddKafkaContainer(p => p with
            {
                Name = name,
                ImageName = imageName,
                Tag = tag,
                EnvironmentVariables = environmentVariables,
                Ports = ports,
                Reusable = reuseContainer
            });

            return builder;
        }

        private static ContainerParameters GetDefaultParameters(this IDockerEnvironmentBuilder builder) =>
            builder.Logger switch
            {
                { } => DefaultParameters with
                {
                    ContainerWaiter = new KafkaContainerWaiter(builder.Logger)
                },
                null => DefaultParameters
            };
    }
}