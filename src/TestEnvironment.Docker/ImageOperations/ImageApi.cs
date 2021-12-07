using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Extensions.Logging;
using static TestEnvironment.Docker.DockerClientExtentions;

namespace TestEnvironment.Docker.ImageOperations
{
    public class ImageApi : IImageApi
    {
        private readonly IDockerClient _dockerClient;
        private readonly IArchiver _archiver;
        private readonly ILogger? _logger;

        public ImageApi()
            : this(CreateDefaultDockerClient(), new Archiver(), null)
        {
        }

        public ImageApi(IDockerClient dockerClient)
            : this(dockerClient, new Archiver(), null)
        {
        }

        public ImageApi(IDockerClient dockerClient, ILogger? logger)
            : this(dockerClient, new Archiver(), logger)
        {
        }

        public ImageApi(ILogger logger)
            : this(CreateDefaultDockerClient(), new Archiver(logger), null)
        {
        }

        public ImageApi(IDockerClient dockerClient, IArchiver archiver, ILogger? logger)
        {
            _dockerClient = dockerClient;
            _archiver = archiver;
            _logger = logger;
        }

#pragma warning disable SA1011 // Closing square brackets should be spaced correctly
        public async Task BuildImageAsync(string dockerfile, string imageName, string tag = "latest", string context = ".", IDictionary<string, string>? buildArgs = null, string[]? ignoredFiles = default, CancellationToken cancellationToken = default)
#pragma warning restore SA1011 // Closing square brackets should be spaced correctly
        {
            var tarFileName = Guid.NewGuid().ToString();

            try
            {
                // In order to pass the context we have to create tar file and use it as an argument.
                await _archiver.CreateTarArchiveAsync(tarFileName, context, ignoredFiles, cancellationToken);

                // Now call docker api.
                await CreateNewImageAsync(dockerfile, imageName, tag, tarFileName, buildArgs, cancellationToken);
            }
            catch (Exception exc)
            {
                _logger?.LogError(exc, $"Unable to create the image from dockerfile.");
                throw;
            }
            finally
            {
                // And don't forget to remove created tar.
                try
                {
                    File.Delete(tarFileName);
                }
                catch (Exception exc)
                {
                    _logger?.LogError(exc, $"Unable to delete tar file {tarFileName} with context. Please, cleanup manually.");
                }
            }
        }

        public async Task PullImageAsync(string imageName, string tag, CancellationToken cancellationToken = default)
        {
            var images = await _dockerClient.Images.ListImagesAsync(
                new ImagesListParameters
                {
                    All = true,
                    Filters = new Dictionary<string, IDictionary<string, bool>>
                    {
                        ["reference"] = new Dictionary<string, bool> { [$"{imageName}:{tag}"] = true }
                    }
                },
                cancellationToken);

            if (!images.Any())
            {
                _logger?.LogInformation($"Pulling the image {imageName}:{tag}");

                // Pull the image.
                try
                {
                    await _dockerClient.Images.CreateImageAsync(
                        new ImagesCreateParameters
                        {
                            FromImage = imageName,
                            Tag = tag
                        },
                        null,
                        new Progress<JSONMessage>(m => _logger?.LogDebug($"Pulling image {imageName}:{tag}:\n{m.ProgressMessage}")),
                        cancellationToken);
                }
                catch (Exception exc)
                {
                    _logger?.LogError(exc, $"Unable to pull the image {imageName}:{tag}");
                    throw;
                }
            }
        }

        private async Task CreateNewImageAsync(string dockerfile, string imageName, string tag, string contextTarFile, IDictionary<string, string>? buildArgs, CancellationToken cancellationToken)
        {
            using var tarContextStream = new FileStream(contextTarFile, FileMode.Open);

            await _dockerClient.Images
                .BuildImageFromDockerfileAsync(
                    new ImageBuildParameters
                    {
                        Dockerfile = dockerfile,
                        BuildArgs = buildArgs ?? new Dictionary<string, string>(),
                        Tags = new[] { $"{imageName}:{tag}" },
                        PullParent = true,
                        Remove = true,
                        ForceRemove = true,
                    },
                    tarContextStream,
                    null,
                    null,
                    new Progress<JSONMessage>(m => _logger?.LogDebug($"Building image {imageName}:{tag}:\n{m.ProgressMessage}")),
                    cancellationToken);
        }
    }
}
