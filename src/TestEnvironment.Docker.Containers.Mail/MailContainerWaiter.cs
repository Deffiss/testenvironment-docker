using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MailKit.Net.Smtp;

namespace TestEnvironment.Docker.Containers.Mail
{
    public class MailContainerWaiter : IContainerWaiter<MailContainer>
    {
        private readonly ushort _smtpPort;
        private readonly ILogger _logger;

        public MailContainerWaiter(ushort smtpPort = 1025, ILogger logger = null)
        {
            _smtpPort = smtpPort;
            _logger = logger;
        }

        public async Task<bool> Wait(MailContainer container, CancellationToken cancellationToken)
        {
            if (container == null) new ArgumentNullException(nameof(container));

            try
            {
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(container.IsDockerInDocker ? container.IPAddress : "localhost", container.IsDockerInDocker ? _smtpPort : container.Ports[_smtpPort]);
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger?.LogDebug(ex.Message);
            }

            return false;
        }

        public Task<bool> Wait(Container container, CancellationToken cancellationToken) => Wait((MailContainer)container, cancellationToken);
    }
}
