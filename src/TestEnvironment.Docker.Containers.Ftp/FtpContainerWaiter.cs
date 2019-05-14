using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using FluentFTP;

namespace TestEnvironment.Docker.Containers.Ftp
{
    public class FtpContainerContainerWaiter : IContainerWaiter<FtpContainer>
    {
        private readonly ILogger _logger;

        public FtpContainerContainerWaiter(ILogger logger = null)
        {
            _logger = logger;
        }

        public async Task<bool> Wait(FtpContainer container, CancellationToken cancellationToken)
        {
            if (container == null) new ArgumentNullException(nameof(container));

            try
            {
                using (var ftpClient = new FtpClient(container.FtpHost, container.IsDockerInDocker ? 21 : container.Ports[21], container.FtpUserName, container.FtpPassword))
                {
                    await ftpClient.ConnectAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger?.LogDebug(ex.Message);
            }

            return false;
        }

        public Task<bool> Wait(Container container, CancellationToken cancellationToken) => Wait((FtpContainer)container, cancellationToken);
    }
}
