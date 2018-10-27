using Docker.DotNet;
using Microsoft.Extensions.Logging;
using Nest;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TestEnvironment.Docker.Containers
{
    public class ElasticsearchContainer : Container, ICleanable
    {
        public ElasticsearchContainer(DockerClient dockerClient, string name, string imageName = "docker.elastic.co/elasticsearch/elasticsearch-oss", string tag = "6.2.4", bool isDockerInDocker = false, bool reuseContainer = false, ILogger logger = null)
            : base(dockerClient, name, imageName, tag,
                new Dictionary<string, string> { ["discovery.type"] = "single-node" },
                isDockerInDocker, new ElasticsearchContainerWaiter(), reuseContainer, logger)
        {
        }

        public async Task Cleanup(CancellationToken token = default)
        {
            var elastic = new ElasticClient(new Uri(GetUrl()));

            await elastic.DeleteIndexTemplateAsync("*");
            await elastic.DeleteIndexAsync("*");
        }

        public string GetUrl() => IsDockerInDocker ? $"http://{IPAddress}:9200" : $"http://localhost:{Ports[9200]}";
    }
}
