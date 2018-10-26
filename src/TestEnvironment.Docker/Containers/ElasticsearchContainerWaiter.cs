using Elasticsearch.Net;
using Nest;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TestEnvironment.Docker.Containers
{
    public class ElasticsearchContainerWaiter : IContainerWaiter<ElasticsearchContainer>
    {
        public async Task<(bool IsReady, string DebugMessage)> Wait(ElasticsearchContainer container, CancellationToken cancellationToken = default)
        {
            if (container == null) new ArgumentNullException(nameof(container));

            var elastic = new ElasticClient(new Uri(container.GetUrl()));
            var health = await elastic.ClusterHealthAsync(ch => ch
                .WaitForStatus(WaitForStatus.Yellow)
                .Level(Level.Cluster)
                .ErrorTrace(true));

            return (health.IsValid, health.DebugInformation);
        }

        public Task<(bool IsReady, string DebugMessage)> Wait(Container container, CancellationToken cancellationToken) => Wait((ElasticsearchContainer)container, cancellationToken);
    }
}