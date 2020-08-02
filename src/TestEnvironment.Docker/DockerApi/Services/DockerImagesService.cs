using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using TestEnvironment.Docker.DockerApi.Helpers;
using TestEnvironment.Docker.DockerApi.Models;

namespace TestEnvironment.Docker.DockerApi.Services
{
    public class DockerImagesService : IDockerImagesService
    {
        private readonly DockerClient _client;

        public DockerImagesService(DockerClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task BuildImage(ImageFromDockerfileConfiguration configuration, string tempFileName, CancellationToken cancellationToken = default)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            using (var tarContextStream = new FileStream(tempFileName, FileMode.Open))
            {
                var image = await _client.Images.BuildImageFromDockerfileAsync(
                    tarContextStream,
                    configuration.ToImageBuildParameters(),
                    cancellationToken);

                await new StreamReader(image).ReadToEndAsync();
            }
        }

        public async Task<bool> IsExists(string imageName, string tag, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(imageName) || string.IsNullOrWhiteSpace(tag))
            {
                throw new ArgumentException("Image Name and Tag shouldn't be null or empty or whitespace!");
            }

            var images = await _client.Images.ListImagesAsync(
                    new ImagesListParameters
                    {
                        All = true,
                        MatchName = $"{imageName}:{tag}"
                    },
                    cancellationToken);

            return images.Any();
        }

        public Task PullImage(string imageName, string tag, Action<string, string, string> progressAction = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(imageName) || string.IsNullOrWhiteSpace(tag))
            {
                throw new ArgumentException("Image Name and Tag shouldn't be null or empty or whitespace!");
            }

            return _client.Images.CreateImageAsync(
                            new ImagesCreateParameters
                            {
                                FromImage = imageName,
                                Tag = tag
                            },
                            null,
                            new Progress<JSONMessage>(m => progressAction?.Invoke(imageName, tag, m.ProgressMessage)),
                            cancellationToken);
        }
    }
}
