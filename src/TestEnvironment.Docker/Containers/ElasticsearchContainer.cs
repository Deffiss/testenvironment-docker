using Docker.DotNet;
using Docker.DotNet.Models;
using Elasticsearch.Net;
using Microsoft.Extensions.Logging;
using Nest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace TestEnvironment.Docker.Containers
{
    public class ElasticsearchContainer : Container
    {
        private const int AttemptsCount = 60;
        private const int DelayTime = 1000;

        public ElasticsearchContainer(DockerClient dockerClient, string name, string imageName = "docker.elastic.co/elasticsearch/elasticsearch-oss", string tag = "6.2.4", ILogger logger = null, bool isDockerInDocker = false)
            : base(dockerClient, name, imageName, tag,
                new Dictionary<string, string> { ["discovery.type"] = "single-node" },
                isDockerInDocker, logger)
        {
        }

        protected override async Task WaitForReadiness(CancellationToken token = default)
        {
            var attempts = AttemptsCount;

            var elastic = new ElasticClient(new Uri(GetUrl()));
            IClusterHealthResponse health;
            do
            {
                health = await elastic.ClusterHealthAsync(ch => ch
                    .WaitForStatus(WaitForStatus.Yellow)
                    .Level(Level.Cluster)
                    .ErrorTrace(true));

                Logger.LogDebug(health.DebugInformation);

                if (!health.IsValid) await Task.Delay(DelayTime);

                var logs = await DockerClient.Containers.GetContainerLogsAsync(Id, new ContainerLogsParameters
                {
                    ShowStderr = true,
                    ShowStdout = true,
                    Tail = "100"
                });

                using (var sr = new StreamReader(logs))
                {
                    Logger.LogDebug($"Container {Id} logs:\n{await sr.ReadToEndAsync()}");
                }

                attempts--;

                Logger.LogDebug($"Attemtps {attempts}");
            } while (!health.IsValid && attempts != 0);

            if (attempts == 0)
            {
                Logger.LogError("Elastic didn't start.");
                throw new TimeoutException("Elastic didn't start.");
            }
        }

        public string GetUrl() => IsDockerInDocker ? $"http://{IPAddress}:9200" : $"http://localhost:{Ports[9200]}";
    }
}
