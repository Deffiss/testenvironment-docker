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
    public class RedisContainerWaiter : BaseContainerWaiter<RedisContainer>
    {
        public RedisContainerWaiter()
        {
        }

        public RedisContainerWaiter(ILogger logger)
            : base(logger)
        {
        }

        protected override async Task<bool> PerformCheckAsync(RedisContainer container, CancellationToken cancellationToken)
        {
            RedisConnectionConfiguration redisConnectionConfiguration = container.GetConnectionConfiguration();

            var redisConfigurationOptions = new ConfigurationOptions()
            {
                EndPoints =
                {
                    { redisConnectionConfiguration.Host, redisConnectionConfiguration.Port },
                },
                Password = redisConnectionConfiguration.Password,
                ConnectTimeout = 2000,
                ConnectRetry = 2
            };

            var redis = await ConnectionMultiplexer.ConnectAsync(redisConfigurationOptions);

            redis.GetStatus();

            await redis.CloseAsync();

            return true;
        }
    }
}
