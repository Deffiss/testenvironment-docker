using Docker.DotNet;
using Microsoft.Extensions.Logging;
using IP = System.Net.IPAddress;

namespace TestEnvironment.Docker.Containers.RabbitMQ
{
    public static class IDockerEnvironmentBuilderExtensions
    {
        public static RabbitMQContainerParameters DefaultParameters => new("rabbitmq", "guest", "guest")
        {
            ImageName = "rabbitmq",
            Tag = "3",
            EnvironmentVariables = new Dictionary<string, string>
            {
                ["RABBITMQ_DEFAULT_USER"] = "guest",
                ["RABBITMQ_DEFAULT_PASS"] = "guest"
            },
            ContainerCleaner = new RabbitMQContainerCleaner(),
            ContainerWaiter = new RabbitMQContainerWaiter()
        };

        public static IDockerEnvironmentBuilder AddRabbitMQContainer(
            this IDockerEnvironmentBuilder builder,
            Func<RabbitMQContainerParameters, RabbitMQContainerParameters> paramsBuilder)
        {
            var parameters = paramsBuilder(builder.GetDefaultParameters());
            builder.AddContainer(parameters, (p, d, l) => new RabbitMQContainer(FixEnvironmentVariables(p), d, l));

            return builder;
        }

        public static IDockerEnvironmentBuilder AddRabbitMQContainer(
            this IDockerEnvironmentBuilder builder,
            Func<RabbitMQContainerParameters, IDockerClient, ILogger?, RabbitMQContainerParameters> paramsBuilder)
        {
            var parameters = paramsBuilder(builder.GetDefaultParameters(), builder.DockerClient, builder.Logger);
            builder.AddContainer(parameters, (p, d, l) => new RabbitMQContainer(FixEnvironmentVariables(p), d, l));

            return builder;
        }

        public static IDockerEnvironmentBuilder AddRabbitMQContainer(
            this IDockerEnvironmentBuilder builder,
            string name,
            string userName,
            string password,
            string imageName = "rabbitmq",
            string tag = "3",
            IDictionary<string, string>? environmentVariables = null,
            IDictionary<ushort, ushort>? ports = null,
            bool reuseContainer = false)
        {
            builder.AddRabbitMQContainer(p => p with
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

        private static RabbitMQContainerParameters FixEnvironmentVariables(RabbitMQContainerParameters p) =>
            p with
            {
                EnvironmentVariables = new Dictionary<string, string>
                {
                    ["RABBITMQ_DEFAULT_USER"] = p.UserName,
                    ["RABBITMQ_DEFAULT_PASS"] = p.Password
                }.MergeDictionaries(p.EnvironmentVariables),
            };

        private static RabbitMQContainerParameters GetDefaultParameters(this IDockerEnvironmentBuilder builder) =>
            builder.Logger switch
            {
                { } => DefaultParameters with
                {
                    ContainerWaiter = new RabbitMQContainerWaiter(builder.Logger),
                    ContainerCleaner = new RabbitMQContainerCleaner(builder.Logger)
                },
                null => DefaultParameters
            };
    }
}