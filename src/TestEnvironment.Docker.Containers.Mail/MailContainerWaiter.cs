using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MailKit.Net.Smtp;

namespace TestEnvironment.Docker.Containers.Mail
{
    public class MailContainerWaiter : BaseContainerWaiter<MailContainer>
    {
        private readonly ushort _smtpPort;

        public MailContainerWaiter(ushort smtpPort = 1025, ILogger logger = null)
            : base(logger)
        {
            _smtpPort = smtpPort;
        }

        protected override async Task<bool> PerformCheck(MailContainer container, CancellationToken cancellationToken)
        {
            try
            {
                using var client = new SmtpClient();
                
                await client.ConnectAsync(container.IsDockerInDocker ? container.IPAddress : "localhost", container.IsDockerInDocker ? _smtpPort : container.Ports[_smtpPort], cancellationToken: cancellationToken);

                return true;
            }
            catch (Exception ex)
            {
                Logger?.LogDebug(ex.Message);
            }

            return false;
        }
    }
}
