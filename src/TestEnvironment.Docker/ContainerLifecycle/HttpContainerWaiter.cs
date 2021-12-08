using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace TestEnvironment.Docker.ContainerLifecycle
{
    public class HttpContainerWaiter : BaseContainerWaiter<Container>
    {
        private readonly string _path;
        private readonly bool _isHttps;
        private readonly ushort _port;
        private readonly HttpStatusCode[] _successfulCodes;

        public HttpContainerWaiter(string path = "", bool isHttps = false, ushort port = 80, HttpStatusCode successfulCode = HttpStatusCode.OK) =>
            (_path, _isHttps, _port, _successfulCodes) = (path, isHttps, port, new[] { successfulCode });

        public HttpContainerWaiter(ILogger logger, string path = "", bool isHttps = false, ushort port = 80, params HttpStatusCode[] successfulCodes)
            : base(logger) =>
            (_path, _isHttps, _port, _successfulCodes) = (path, isHttps, port, successfulCodes);

        protected override async Task<bool> PerformCheckAsync(Container container, CancellationToken cancellationToken)
        {
            var uri = new Uri($"{(_isHttps ? "https" : "http")}://" +
                              $"{(container.IsDockerInDocker ? container.IPAddress : IPAddress.Loopback.ToString())}:" +
                              $"{(container.IsDockerInDocker ? _port : container.Ports?[_port])}");

            using var client = new HttpClient { BaseAddress = uri };
            var response = await client.GetAsync(_path, cancellationToken);

            return _successfulCodes.Contains(response.StatusCode);
        }
    }
}
