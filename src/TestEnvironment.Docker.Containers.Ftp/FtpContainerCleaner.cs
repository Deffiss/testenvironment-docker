using System;
using System.Threading;
using System.Threading.Tasks;
using FluentFTP;
using Microsoft.Extensions.Logging;
using TestEnvironment.Docker.ContainerLifecycle;

namespace TestEnvironment.Docker.Containers.Ftp
{
    public class FtpContainerCleaner : IContainerCleaner<FtpContainer>
    {
        private readonly ILogger? _logger;

        public FtpContainerCleaner()
        {
        }

        public FtpContainerCleaner(ILogger logger)
        {
            _logger = logger;
        }

        public async Task CleanupAsync(FtpContainer container, CancellationToken cancellationToken = default)
        {
            using var ftpClient = new FtpClient(container.FtpHost, container.IsDockerInDocker ? 21 : container.Ports![21], container.FtpUserName, container.FtpPassword);
            try
            {
                await ftpClient.ConnectAsync(cancellationToken);
                foreach (var item in await ftpClient.GetListingAsync("/"))
                {
                    if (item.Type == FtpFileSystemObjectType.Directory)
                    {
                        await ftpClient.DeleteDirectoryAsync(item.FullName, cancellationToken);
                    }
                    else if (item.Type == FtpFileSystemObjectType.File)
                    {
                        await ftpClient.DeleteFileAsync(item.FullName, cancellationToken);
                    }
                }
            }
            catch (Exception e)
            {
                _logger?.LogInformation($"Cleanup issue: {e.Message}");
            }
        }

        public Task CleanupAsync(Container container, CancellationToken cancellationToken = default) =>
            CleanupAsync((FtpContainer)container, cancellationToken);
    }
}
