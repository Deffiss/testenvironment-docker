using System.Collections.Generic;
using Docker.DotNet;
using Microsoft.Extensions.Logging;
using TestEnvironment.Docker.ContainerOperations;
using TestEnvironment.Docker.ImageOperations;
using IP = System.Net.IPAddress;

namespace TestEnvironment.Docker.Containers.Kafka
{
    public class KafkaContainer : Container
    {
        public KafkaContainer(ContainerParameters containerParameters)
            : base(containerParameters)
        {
        }

        public KafkaContainer(ContainerParameters containerParameters, IDockerClient dockerClient)
            : base(containerParameters, dockerClient)
        {
        }

        public KafkaContainer(ContainerParameters containerParameters, IDockerClient dockerClient, ILogger? logger)
            : base(containerParameters, dockerClient, logger)
        {
        }

        public KafkaContainer(ContainerParameters containerParameters, IContainerApi containerApi, ImageApi imageApi, ILogger? logger)
            : base(containerParameters, containerApi, imageApi, logger)
        {
        }

        public string GetUrl() => IsDockerInDocker ? $"{IPAddress}:9092" : $"{IP.Loopback}:{Ports![9092]}";
    }
}