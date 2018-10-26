using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TestEnvironment.Docker
{
    public class DockerEnvironment : ITestEnvironment
    {
        private readonly DockerClient _dockerClient;
        private readonly ILogger _logger;

        public string Name { get; }

        public IDictionary<string, string> Variables { get; }

        public IDependency[] Dependencies { get; }

        public DockerEnvironment(string name, IDictionary<string, string> variables, IDependency[] dependencies, DockerClient dockerClient, ILogger logger = null)
        {
            Name = name;
            Variables = variables;
            Dependencies = dependencies;
            _dockerClient = dockerClient;
            _logger = logger;
        }

        public async Task Up(CancellationToken token = default)
        {
            await PullRequiredImages(token);

            await Task.WhenAll(Dependencies.Select(d => d.Run(Variables, token)));
        }

        public Task Down(CancellationToken token = default) =>
            Task.WhenAll(Dependencies.Select(d => d.Stop(token)));

        public Container GetContainer(string name) =>
            Dependencies.FirstOrDefault(d => d is Container c && c.Name.Equals(name.GetContainerName(Name), StringComparison.OrdinalIgnoreCase)) as Container;

        public TContainer GetContainer<TContainer>(string name) where TContainer : Container => GetContainer(name) as TContainer;

        public void Dispose()
        {
            foreach (var dependency in Dependencies)
            {
                dependency.Dispose();
            }
        }

        private async Task PullRequiredImages(CancellationToken token)
        {
            foreach(var contianer in Dependencies.OfType<Container>())
            {
                var images = await _dockerClient.Images.ListImagesAsync(new ImagesListParameters
                {
                    All = true,
                    MatchName = $"{contianer.ImageName}:{contianer.Tag}"
                }, token);

                if (!images.Any())
                {
                    _logger.LogInformation($"Pulling the image {contianer.ImageName}:{contianer.Tag}");

                    // Pull the image.
                    await _dockerClient.Images.CreateImageAsync(
                        new ImagesCreateParameters
                        {
                            FromImage = contianer.ImageName,
                            Tag = contianer.Tag
                        }, null, new Progress<JSONMessage>(m => _logger.LogDebug($"Pulling image {contianer.ImageName}:{contianer.Tag}:\n{m.ProgressMessage}")), token);
                }
            }
        }
    }
}
