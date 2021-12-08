using System;
using System.Threading;
using System.Threading.Tasks;
using Nest;
using TestEnvironment.Docker.ContainerLifecycle;

namespace TestEnvironment.Docker.Containers.Elasticsearch
{
    public class ElasticsearchContainerCleaner : IContainerCleaner<ElasticsearchContainer>
    {
        public async Task CleanupAsync(ElasticsearchContainer container, CancellationToken cancellationToken = default)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            var elastic = new ElasticClient(new Uri(container.GetUrl()));

            await elastic.Indices.DeleteTemplateAsync("*", ct: cancellationToken);
            await elastic.Indices.DeleteAsync("*", ct: cancellationToken);
        }

        public Task CleanupAsync(Container container, CancellationToken cancellationToken = default) =>
            CleanupAsync((ElasticsearchContainer)container, cancellationToken);
    }
}
