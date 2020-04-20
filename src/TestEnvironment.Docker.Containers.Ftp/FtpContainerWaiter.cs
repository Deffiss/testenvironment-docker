using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using FluentFTP;

namespace TestEnvironment.Docker.Containers.Ftp
{
    public class FtpContainerContainerWaiter : BaseContainerWaiter<FtpContainer>
    {
        public FtpContainerContainerWaiter(ILogger logger = null)
            : base(logger)
        {
        }

        protected override async Task<bool> PerformCheck(FtpContainer container, CancellationToken cancellationToken)
        {
            using var ftpClient = new FtpClient(
                container.FtpHost,
                container.IsDockerInDocker ? 21 : container.Ports[21], container.FtpUserName,
                container.FtpPassword);
                
            await ftpClient.ConnectAsync(cancellationToken);

            return true;
        }
    }
}
