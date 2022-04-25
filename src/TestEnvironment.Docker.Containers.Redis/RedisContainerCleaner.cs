using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using TestEnvironment.Docker.ContainerLifecycle;

namespace TestEnvironment.Docker.Containers.Redis
{
    public class RedisContainerCleaner : IContainerCleaner<RedisContainer>
    {
        private readonly ILogger? _logger;

        public RedisContainerCleaner()
        {
        }

        public RedisContainerCleaner(ILogger logger) =>
            _logger = logger;

        public async Task CleanupAsync(RedisContainer container, CancellationToken cancellationToken = default)
        {
            RedisConnectionConfiguration redisConnectionConfiguration = container.GetConnectionConfiguration();

            var redisConfigurationOptions = new ConfigurationOptions()
            {
                EndPoints =
                {
                    { redisConnectionConfiguration.Host, redisConnectionConfiguration.Port },
                },
                Password = redisConnectionConfiguration.Password,
                AllowAdmin = true
            };

            ConnectionMultiplexer redis = await ConnectionMultiplexer.ConnectAsync(redisConfigurationOptions);

            try
            {
                await redis.GetServer(redisConnectionConfiguration.Host).FlushAllDatabasesAsync();
                await redis.CloseAsync();
            }
            catch (Exception e)
            {
                _logger?.LogError($"Redis cleanup error: {e.Message}");
            }
        }

        public Task CleanupAsync(Container container, CancellationToken cancellationToken = default) => CleanupAsync((RedisContainer)container, cancellationToken);
    }
}
