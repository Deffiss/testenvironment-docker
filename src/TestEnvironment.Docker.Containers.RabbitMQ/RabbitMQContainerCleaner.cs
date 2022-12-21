using Microsoft.Extensions.Logging;
using TestEnvironment.Docker.ContainerLifecycle;

namespace TestEnvironment.Docker.Containers.RabbitMQ
{
    public class RabbitMQContainerCleaner : IContainerCleaner<RabbitMQContainer>
    {
        private readonly ILogger? _logger;

        public RabbitMQContainerCleaner()
        {
        }

        public RabbitMQContainerCleaner(ILogger logger) =>
            _logger = logger;

        public async Task CleanupAsync(RabbitMQContainer container, CancellationToken cancellationToken = default)
        {
            try
            {
                await container.ExecAsync(
                    new[]
                    {
                        "rabbitmqctl",
                        "stop_app"
                    },
                    cancellationToken: cancellationToken);

                await container.ExecAsync(
                    new[]
                    {
                        "rabbitmqctl",
                        "reset"
                    },
                    cancellationToken: cancellationToken);

                await container.ExecAsync(
                    new[]
                    {
                        "rabbitmqctl",
                        "start_app"
                    },
                    cancellationToken: cancellationToken);
            }
            catch (Exception e)
            {
                _logger?.LogInformation($"Cleanup issue: {e.Message}");
            }
        }

        public Task CleanupAsync(Container container, CancellationToken cancellationToken = default) =>
            CleanupAsync((RabbitMQContainer)container, cancellationToken);
    }
}