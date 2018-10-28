using Elasticsearch.Net;
using Microsoft.Extensions.Logging;
using Nest;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TestEnvironment.Docker.Containers
{
    public class ElasticsearchContainerWaiter : IContainerWaiter<ElasticsearchContainer>
    {
        private readonly ILogger _logger;

        public ElasticsearchContainerWaiter(ILogger logger = null)
        {
            _logger = logger;
        }

        public async Task<bool> Wait(ElasticsearchContainer container, CancellationToken cancellationToken = default)
        {
            if (container == null) new ArgumentNullException(nameof(container));

            var elastic = new ElasticClient(new Uri(container.GetUrl()));
            var health = await elastic.ClusterHealthAsync(ch => ch
                .WaitForStatus(WaitForStatus.Yellow)
                .Level(Level.Cluster)
                .ErrorTrace(true));

            _logger?.LogDebug(health.DebugInformation);

            return health.IsValid;
        }

        public Task<bool> Wait(Container container, CancellationToken cancellationToken) => Wait((ElasticsearchContainer)container, cancellationToken);
    }
}