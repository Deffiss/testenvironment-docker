using Nest;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TestEnvironment.Docker.Containers
{
    public class ElasticsearchContainerCleaner : IContainerCleaner<ElasticsearchContainer>
    {
        public async Task Cleanup(ElasticsearchContainer container, CancellationToken token = default)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            var elastic = new ElasticClient(new Uri(container.GetUrl()));

            await elastic.DeleteIndexTemplateAsync("*");
            await elastic.DeleteIndexAsync("*");
        }

        public Task Cleanup(Container container, CancellationToken token = default) => Cleanup((ElasticsearchContainer)container, token);
    }
}
