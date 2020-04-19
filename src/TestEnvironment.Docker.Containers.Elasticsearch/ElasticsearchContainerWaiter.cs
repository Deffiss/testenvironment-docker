using Elasticsearch.Net;
using Microsoft.Extensions.Logging;
using Nest;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TestEnvironment.Docker.Containers.Elasticsearch
{
    public class ElasticsearchContainerWaiter : BaseContainerWaiter<ElasticsearchContainer>
    {
        public ElasticsearchContainerWaiter(ILogger logger = null)
            : base(logger)
        {
        }

        protected override async Task<bool> PerformCheckAsync(ElasticsearchContainer container,
            CancellationToken cancellationToken)
        {
            var elastic = new ElasticClient(new Uri(container.GetUrl()));
            var health = await elastic.ClusterHealthAsync(ch => ch
                .WaitForStatus(WaitForStatus.Yellow)
                .Level(Level.Cluster)
                .ErrorTrace(true), cancellationToken);

            Logger?.LogDebug(health.DebugInformation);

            return health.IsValid;
        }
    }
}
