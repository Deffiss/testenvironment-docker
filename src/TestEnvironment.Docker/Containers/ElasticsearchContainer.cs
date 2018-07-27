using Docker.DotNet;
using Docker.DotNet.Models;
using Elasticsearch.Net;
using Nest;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace TestEnvironment.Docker.Containers
{
    public class ElasticsearchContainer : Container
    {
        private const int AttemptsCount = 60;
        private const int DelayTime = 1000;

        public ElasticsearchContainer(DockerClient dockerClient, string name, string imageName = "docker.elastic.co/elasticsearch/elasticsearch-oss", string tag = "6.2.4", Action<string> logger = null, bool isDockerInDocker = false)
            : base(dockerClient, name, imageName, tag,
                new[] { ("discovery.type", "single-node") },
                logger, isDockerInDocker)
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

                Console.WriteLine(health.DebugInformation);

                if (!health.IsValid) await Task.Delay(DelayTime);

                var logs = await DockerClient.Containers.GetContainerLogsAsync(Id, new ContainerLogsParameters
                {
                    ShowStderr = true,
                    ShowStdout = true,
                    Tail = "100"
                });

                using (var sr = new StreamReader(logs))
                {
                    Console.WriteLine($"Container {Id} logs:\n{await sr.ReadToEndAsync()}");
                }

                attempts--;

                Console.WriteLine($"Attemtps {attempts}");
            } while (!health.IsValid && attempts != 0);

            if (attempts == 0) throw new TimeoutException("Elastic didn't start");
        }

        public string GetUrl() => IsDockerInDocker ? $"http://{IPAddress}:9200" : $"http://localhost:{Ports[9200]}";
    }
}
