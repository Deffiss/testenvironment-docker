using System;
using System.Collections.Generic;
using Docker.DotNet;
using Microsoft.Extensions.Logging;

namespace TestEnvironment.Docker.Containers.Kafka
{
    public static class IDockerEnvironmentBuilderExtensions
    {
        private static ContainerParameters DefaultParameters => new ("kafka", "johnnypark/kafka-zookeeper")
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
            ContainerCleaner = new KafkaContainerCleaner(),
            ContainerWaiter = new KafkaContainerWaiter()
        };

        public static IDockerEnvironmentBuilder AddKafkaContainer(
            this IDockerEnvironmentBuilder builder,
            Func<ContainerParameters, ContainerParameters> paramsBuilder)
        {
            var parameters = paramsBuilder(builder.GetDefaultParameters());
            builder.AddContainer(builder.GetDefaultParameters(), (p, d, l) => new Container(p, d, l));

            return builder;
        }

        public static IDockerEnvironmentBuilder AddKafkaContainer(
            this IDockerEnvironmentBuilder builder,
            Func<ContainerParameters, IDockerClient, ILogger?, ContainerParameters> paramsBuilder)
        {
            var parameters = paramsBuilder(builder.GetDefaultParameters(), builder.DockerClient, builder.Logger);
            builder.AddContainer(parameters, (p, d, l) => new Container(p, d, l));

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