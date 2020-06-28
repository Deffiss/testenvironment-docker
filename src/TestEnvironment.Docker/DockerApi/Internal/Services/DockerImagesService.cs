using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using TestEnvironment.Docker.DockerApi.Abstractions.Models;
using TestEnvironment.Docker.DockerApi.Abstractions.Services;

namespace TestEnvironment.Docker.DockerApi.Internal.Services
{
    public class DockerImagesService : IDockerImagesService
    {
        private readonly DockerClient _client;

        public DockerImagesService(DockerClient client)
        {
            _client = client;
        }

        public async Task BuildImage(ImageFromDockerfileConfiguration configuration, string tempFileName, CancellationToken cancellationToken)
        {
            using (var tarContextStream = new FileStream(tempFileName, FileMode.Open))
            {
                var image = await _client.Images.BuildImageFromDockerfileAsync(
                    tarContextStream,
                    configuration.ToImageBuildParameters(),
                    cancellationToken);

                await new StreamReader(image).ReadToEndAsync();
            }
        }

        public async Task<bool> IsExists(string imageName, string tag, CancellationToken cancellationToken)
        {
            var images = await _client.Images.ListImagesAsync(
                    new ImagesListParameters
                    {
                        All = true,
                        MatchName = $"{imageName}:{tag}"
                    },
                    cancellationToken);

            return images.Any();
        }

        public Task PullImage(string imageName, string tag, Action<string, string, string> progressAction, CancellationToken cancellationToken)
        {
            return _client.Images.CreateImageAsync(
                            new ImagesCreateParameters
                            {
                                FromImage = imageName,
                                Tag = tag
                            },
                            null,
                            new Progress<JSONMessage>(m => progressAction(imageName, tag, m.ProgressMessage)),
                            cancellationToken);
        }
    }
}
