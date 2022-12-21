using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using TestEnvironment.Docker.ContainerLifecycle;

namespace TestEnvironment.Docker.Containers.RabbitMQ
{
    public class RabbitMQContainerWaiter : BaseContainerWaiter<RabbitMQContainer>
    {
        public RabbitMQContainerWaiter()
        {
        }

        public RabbitMQContainerWaiter(ILogger logger)
            : base(logger)
        {
        }

        protected override Task<bool> PerformCheckAsync(RabbitMQContainer container, CancellationToken cancellationToken)
        {
            var factory = new ConnectionFactory
            {
                HostName = container.Host,
                Port = container.Port,
                UserName = container.UserName,
                Password = container.Password
            };

            using var connection = factory.CreateConnection();

            return Task.FromResult(connection.IsOpen);
        }
    }
}