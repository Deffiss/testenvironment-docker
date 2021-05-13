using System.Collections.Generic;
using Docker.DotNet;
using Microsoft.Extensions.Logging;
using TestEnvironment.Docker.ContainerOperations;
using TestEnvironment.Docker.ImageOperations;
using IP = System.Net.IPAddress;

namespace TestEnvironment.Docker.Containers.Elasticsearch
{
    public class ElasticsearchContainer : Container
    {
        public ElasticsearchContainer(ContainerParameters containerParameters)
            : base(containerParameters)
        {
        }

        public ElasticsearchContainer(ContainerParameters containerParameters, IDockerClient dockerClient)
            : base(containerParameters, dockerClient)
        {
        }

        public ElasticsearchContainer(ContainerParameters containerParameters, IDockerClient dockerClient, ILogger? logger)
            : base(containerParameters, dockerClient, logger)
        {
        }

        public ElasticsearchContainer(ContainerParameters containerParameters, IContainerApi containerApi, ImageApi imageApi, ILogger? logger)
            : base(containerParameters, containerApi, imageApi, logger)
        {
        }

        public string GetUrl() => IsDockerInDocker ? $"http://{IPAddress}:9200" : $"http://{IP.Loopback}:{Ports![9200]}";
    }
}
