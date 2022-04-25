using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Docker.DotNet;
using Microsoft.Extensions.Logging;

namespace TestEnvironment.Docker.Containers.Redis
{
    public static class IDockerEnvironmentBuilderExtensions
    {
        public static RedisContainerParameters DefaultParameters => new("redis", "password")
        {
            ImageName = "redis",
            ContainerCleaner = new RedisContainerCleaner(),
            ContainerWaiter = new RedisContainerWaiter(),
        };

        public static IDockerEnvironmentBuilder AddRedisContainer(
            this IDockerEnvironmentBuilder builder,
            Func<RedisContainerParameters, RedisContainerParameters> paramsBuilder)
        {
            var parameters = paramsBuilder(builder.GetDefaultRedisContainerParameters());
            builder.AddContainer(parameters, (p, d, l) => new RedisContainer(FixRedisEnvironmentVariables(p), d, l));

            return builder;
        }

        public static IDockerEnvironmentBuilder AddRedisContainer(
            this IDockerEnvironmentBuilder builder,
            Func<RedisContainerParameters, IDockerClient, ILogger?, RedisContainerParameters> paramsBuilder)
        {
            var parameters = paramsBuilder(builder.GetDefaultRedisContainerParameters(), builder.DockerClient, builder.Logger);
            builder.AddContainer(parameters, (p, d, l) => new RedisContainer(FixRedisEnvironmentVariables(p), d, l));

            return builder;
        }

        private static RedisContainerParameters FixRedisEnvironmentVariables(RedisContainerParameters p) =>
            p with
            {
                EnvironmentVariables = new Dictionary<string, string>
                {
                    ["REDIS_PASSWORD"] = p.Password
                }.MergeDictionaries(p.EnvironmentVariables),
            };

        private static RedisContainerParameters GetDefaultRedisContainerParameters(this IDockerEnvironmentBuilder builder) =>
            builder.Logger switch
            {
                { } => DefaultParameters with
                {
                    ContainerWaiter = new RedisContainerWaiter(builder.Logger),
                    ContainerCleaner = new RedisContainerCleaner(builder.Logger)
                },
                null => DefaultParameters
            };
    }
}
