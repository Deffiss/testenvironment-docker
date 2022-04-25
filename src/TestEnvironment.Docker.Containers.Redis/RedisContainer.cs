using Docker.DotNet;
using Microsoft.Extensions.Logging;
using TestEnvironment.Docker.ContainerOperations;
using TestEnvironment.Docker.ImageOperations;
using IP = System.Net.IPAddress;

namespace TestEnvironment.Docker.Containers.Redis
{
    public class RedisContainer : Container, IRedisContainer
    {
        private readonly RedisContainerParameters _parameters;

        public RedisContainer(RedisContainerParameters containerParameters)
            : base(containerParameters)
        {
            _parameters = containerParameters;
        }

        public RedisContainer(RedisContainerParameters containerParameters, IDockerClient dockerClient)
            : base(containerParameters, dockerClient)
        {
            _parameters = containerParameters;
        }

        public RedisContainer(RedisContainerParameters containerParameters, IDockerClient dockerClient, ILogger? logger)
            : base(containerParameters, dockerClient, logger)
        {
            _parameters = containerParameters;
        }

        public RedisContainer(RedisContainerParameters containerParameters, IContainerApi containerApi, ImageApi imageApi, ILogger? logger)
            : base(containerParameters, containerApi, imageApi, logger)
        {
            _parameters = containerParameters;
        }

        public RedisConnectionConfiguration GetConnectionConfiguration()
        {
            var dindIpAddress = IsDockerInDocker ? IPAddress : null;
            string hostname = dindIpAddress ?? IP.Loopback.ToString();

            var port = IsDockerInDocker ? 6379 : Ports![6379];

            return new RedisConnectionConfiguration(hostname, port, _parameters.Password);
        }
    }
}
