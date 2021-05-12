using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TestEnvironment.Docker.ContainerLifecycle;

namespace TestEnvironment.Docker.Containers.Mail
{
    public class MailContainerCleaner : IContainerCleaner<MailContainer>
    {
        private readonly ushort _apiPort;
        private readonly string _deleteEndpoint;
        private readonly ILogger? _logger;

        public MailContainerCleaner(ushort apiPort = 8025, string deleteEndpoint = "api/v1/messages") =>
            (_apiPort, _deleteEndpoint) = (apiPort, deleteEndpoint);

        public MailContainerCleaner(ILogger logger, ushort apiPort = 8025, string deleteEndpoint = "api/v1/messages") =>
            (_logger, _apiPort, _deleteEndpoint) = (logger, apiPort, deleteEndpoint);

        public async Task CleanupAsync(MailContainer container, CancellationToken cancellationToken = default)
        {
            var uri = new Uri($"http://" +
                $"{(container.IsDockerInDocker ? container.IPAddress : IPAddress.Loopback.ToString())}:" +
                $"{(container.IsDockerInDocker ? _apiPort : container.Ports![_apiPort])}");

            using (var httpClient = new HttpClient { BaseAddress = uri })
            {
                try
                {
                    var response = await httpClient.DeleteAsync(_deleteEndpoint);

                    if (!response.IsSuccessStatusCode)
                    {
                        _logger?.LogWarning($"Cleanup issue: server replied with {response.StatusCode} code.");
                    }
                }
                catch (Exception e)
                {
                    _logger?.LogWarning($"Cleanup issue: {e.Message}");
                }
            }
        }

        public Task CleanupAsync(Container container, CancellationToken cancellationToken = default) =>
            CleanupAsync((MailContainer)container, cancellationToken);
    }
}
