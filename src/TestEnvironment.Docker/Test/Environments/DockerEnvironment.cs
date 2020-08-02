using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SharpCompress.Common;
using SharpCompress.Writers;
using TestEnvironment.Docker.DockerApi.Models;
using TestEnvironment.Docker.DockerApi.Services;
using TestEnvironment.Docker.Test.Helpers;

namespace TestEnvironment.Docker.Test.Environments
{
    public class DockerEnvironment
    {
        private readonly DockerEnvironmentConfiguration _configuration;
        private readonly IDockerImagesService _dockerImagesService;

        public DockerEnvironment(DockerEnvironmentConfiguration configuration)
        {
            _configuration = configuration;
            _dockerImagesService = new DockerImagesService(DockerClientStorage.DockerClient);
        }

        public bool IsDockerInDocker => _configuration.IsDockerInDocker;

        public static DockerEnvironmentBuilder Create()
        {
            return new DockerEnvironmentBuilder();
        }

        public async Task Up(CancellationToken cancellationToken = default)
        {
            await BuildRequiredImages(cancellationToken);
            await PullRequiredImages(cancellationToken);
            await Task.WhenAll(_configuration.Containers.Select(container => RunContainerAndLogResult(container, cancellationToken)));
        }

        public Task Down(CancellationToken cancellationToken = default)
        {
            return Task.WhenAll(_configuration.Containers.Select(container => container.Stop(cancellationToken)));
        }

        private async Task BuildRequiredImages(CancellationToken token)
        {
            foreach (var container in _configuration.Containers.OfType<ContainerFromDockerfile>())
            {
                var tempFileName = Guid.NewGuid().ToString();
                var contextDirectory = container.Context.Equals(".") ? Directory.GetCurrentDirectory() : container.Context;

                try
                {
                    // In order to pass the context we have to create tar file and use it as an argument.
                    CreateTarArchive(_configuration.IgnoredFolders, tempFileName, contextDirectory, _configuration.Logger);

                    // Now call docker api.
                    var configuration = new ImageFromDockerfileConfiguration(container.ImageName, container.Tag, container.Dockerfile, container.BuildArgs);
                    await _dockerImagesService.BuildImage(configuration, tempFileName, token);
                }
                catch (Exception exc)
                {
                    _configuration.Logger.LogError(exc, $"Unable to create the image from dockerfile.");
                    throw;
                }

                // And don't forget to remove created tar.
                try
                {
                    File.Delete(tempFileName);
                }
                catch (Exception exc)
                {
                    _configuration.Logger.LogError(exc, $"Unable to delete tar file {tempFileName} with context. Please, cleanup manually.");
                }

                static void CreateTarArchive(string[] ignoredFolders, string tempFileName, string contextDirectory, ILogger logger)
                {
                    using (var stream = File.OpenWrite(tempFileName))
                    using (var writer = WriterFactory.Open(stream, ArchiveType.Tar, CompressionType.None))
                    {
                        AddDirectoryFilesToTar(writer, contextDirectory, true, ignoredFolders, tempFileName, contextDirectory, logger);
                    }
                }

                // Adds recuresively files to tar archive.
                static void AddDirectoryFilesToTar(IWriter writer, string sourceDirectory, bool recurse, string[] ignoredFolders, string tempFileName, string contextDirectory, ILogger logger)
                {
                    if (ignoredFolders?.Any(excl => excl.Equals(Path.GetFileName(sourceDirectory))) == true)
                    {
                        return;
                    }

                    // Write each file to the tar.
                    var filenames = Directory.GetFiles(sourceDirectory);
                    foreach (string filename in filenames)
                    {
                        if (Path.GetFileName(filename).Equals(tempFileName))
                        {
                            continue;
                        }

                        if (new FileInfo(filename).Attributes.HasFlag(FileAttributes.Hidden))
                        {
                            continue;
                        }

                        // Make sure that we can read the file
                        try
                        {
                            File.OpenRead(filename);
                        }
                        catch (Exception)
                        {
                            continue;
                        }

                        try
                        {
                            var contextDirectoryIndex = filename.IndexOf(contextDirectory);
                            var cleanPath = (contextDirectoryIndex < 0)
                                ? filename
                                : filename.Remove(contextDirectoryIndex, contextDirectory.Length);

                            writer.Write(cleanPath, filename);
                        }
                        catch (Exception exc)
                        {
                            logger.LogWarning($"Can not add file {filename} to the context: {exc.Message}.");
                        }
                    }

                    if (recurse)
                    {
                        var directories = Directory.GetDirectories(sourceDirectory);
                        foreach (var directory in directories)
                        {
                            AddDirectoryFilesToTar(writer, directory, recurse, ignoredFolders, tempFileName, contextDirectory, logger);
                        }
                    }
                }
            }
        }

        private async Task PullRequiredImages(CancellationToken token)
        {
            foreach (var contianer in _configuration.Containers)
            {
                var imageExists = await _dockerImagesService.IsExists(contianer.ImageName, contianer.Tag, token);

                if (!imageExists)
                {
                    _configuration.Logger.LogInformation($"Pulling the image {contianer.ImageName}:{contianer.Tag}");

                    // Pull the image.
                    try
                    {
                        await _dockerImagesService.PullImage(
                            contianer.ImageName,
                            contianer.Tag,
                            (imageName, tag, message) => _configuration.Logger.LogDebug($"Pulling image {imageName}:{tag}:\n{message}"),
                            token);
                    }
                    catch (Exception e)
                    {
                        _configuration.Logger.LogError(e, $"Unable to pull the image {contianer.ImageName}:{contianer.Tag}");
                        throw;
                    }
                }
            }
        }

        private async Task RunContainerAndLogResult(Containers.Container container, CancellationToken cancellationToken)
        {
            var logger = _configuration.Logger;
            try
            {
                var result = await container.Run(_configuration.EnvironmentVariables, cancellationToken);
                logger.LogInformation($"Container '{container.Name}' has been run.");
                logger.LogDebug($"Container state: {result.State}");
                logger.LogDebug($"Container status: {result.Status}");
                logger.LogDebug($"Container IPAddress: {result.NetworkName} - {result.IPAddress}");
                container.AssignDockerEnvironment(this);
            }
            catch (Exception e)
            {
                logger.LogError($"Container {container.Name} didn't start. Exception: {e}");
            }
        }
    }
}
