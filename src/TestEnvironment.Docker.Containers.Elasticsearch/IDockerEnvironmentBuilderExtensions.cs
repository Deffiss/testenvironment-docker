using System;
using System.Collections.Generic;
using Docker.DotNet;
using Microsoft.Extensions.Logging;

namespace TestEnvironment.Docker.Containers.Elasticsearch
{
    public static class IDockerEnvironmentBuilderExtensions
    {
        public static ContainerParameters DefaultParameters => new("elastic", "docker.elastic.co/elasticsearch/elasticsearch-oss")
        {
            Tag = "7.0.1",
            EnvironmentVariables = new Dictionary<string, string>
            {
                ["discovery.type"] = "single-node",
            },
            ContainerCleaner = new ElasticsearchContainerCleaner(),
            ContainerWaiter = new ElasticsearchContainerWaiter()
        };

        public static IDockerEnvironmentBuilder AddElasticsearchContainer(
            this IDockerEnvironmentBuilder builder,
            Func<ContainerParameters, ContainerParameters> paramsBuilder)
        {
            var parameters = paramsBuilder(builder.GetDefaultParameters());
            builder.AddContainer(parameters, (p, d, l) => new ElasticsearchContainer(p, d, l));

            return builder;
        }

        public static IDockerEnvironmentBuilder AddElasticsearchContainer(
            this IDockerEnvironmentBuilder builder,
            Func<ContainerParameters, IDockerClient, ILogger?, ContainerParameters> paramsBuilder)
        {
            var parameters = paramsBuilder(builder.GetDefaultParameters(), builder.DockerClient, builder.Logger);
            builder.AddContainer(parameters, (p, d, l) => new ElasticsearchContainer(p, d, l));

            return builder;
        }

        [Obsolete("This method is depricated and will be removed in upcoming versions.")]
        public static IDockerEnvironmentBuilder AddElasticsearchContainer(
            this IDockerEnvironmentBuilder builder,
            string name,
            string imageName = "docker.elastic.co/elasticsearch/elasticsearch-oss",
            string tag = "7.0.1",
            IDictionary<string, string>? environmentVariables = null,
            IDictionary<ushort, ushort>? ports = null,
            bool reuseContainer = false)
        {
            builder.AddElasticsearchContainer(p => p with
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
                    ContainerWaiter = new ElasticsearchContainerWaiter(builder.Logger)
                },
                null => DefaultParameters
            };
    }
}
