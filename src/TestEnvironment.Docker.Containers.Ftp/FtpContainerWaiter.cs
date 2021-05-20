using System.Threading;
using System.Threading.Tasks;
using FluentFTP;
using Microsoft.Extensions.Logging;
using TestEnvironment.Docker.ContainerLifecycle;

namespace TestEnvironment.Docker.Containers.Ftp
{
    public class FtpContainerWaiter : BaseContainerWaiter<FtpContainer>
    {
        public FtpContainerWaiter()
        {
        }

        public FtpContainerWaiter(ILogger logger)
            : base(logger)
        {
        }

        protected override async Task<bool> PerformCheckAsync(FtpContainer container, CancellationToken cancellationToken)
        {
            using var ftpClient = new FtpClient(
                container.FtpHost,
                container.IsDockerInDocker ? 21 : container.Ports![21],
                container.FtpUserName,
                container.FtpPassword);

            await ftpClient.ConnectAsync(cancellationToken);

            return true;
        }
    }
}
