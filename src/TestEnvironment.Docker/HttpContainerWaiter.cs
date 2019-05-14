using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TestEnvironment.Docker
{
    public class HttpContainerWaiter : IContainerWaiter
    {
        private readonly string _path;
        private readonly bool _isHttps;
        private readonly ushort _httpPort;
        private readonly ILogger _logger;
        private readonly HttpStatusCode[] _successfulCodes;

        public HttpContainerWaiter(string path, bool isHttps = false, ushort httpPort = 80, ILogger logger = null, HttpStatusCode successfulCode = HttpStatusCode.OK)
            : this(path, isHttps, httpPort, logger, new[] { successfulCode })
        {
        }

        public HttpContainerWaiter(string path, bool isHttps = false, ushort httpPort = 80, ILogger logger = null, params HttpStatusCode[] successfulCodes)
        {
            _path = path;
            _isHttps = isHttps;
            _httpPort = httpPort;
            _logger = logger;
            _successfulCodes = successfulCodes;
        }

        public async Task<bool> Wait(Container container, CancellationToken cancellationToken)
        {
            if (container is null) throw new ArgumentNullException(nameof(container));

            try
            {
                var uri = new Uri($"{(_isHttps ? "https" : "http")}://" +
                    $"{(container.IsDockerInDocker ? container.IPAddress : "localhost")}:" +
                    $"{(container.IsDockerInDocker ? _httpPort : container.Ports[_httpPort])}");

                using (var client = new HttpClient { BaseAddress = uri })
                {
                    var response = await client.GetAsync(_path);

                    return _successfulCodes.Contains(response.StatusCode);
                }
            }
            catch (Exception e)
            {
                _logger?.LogDebug(e.Message);
            }

            return false;
        }
    }
}
