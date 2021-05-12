using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet;
using Microsoft.Extensions.Logging;
using TestEnvironment.Docker.ContainerOperations;
using TestEnvironment.Docker.ImageOperations;
using static TestEnvironment.Docker.DockerClientExtentions;
using static TestEnvironment.Docker.StringExtensions;

namespace TestEnvironment.Docker
{
    public class DockerEnvironment : IDockerEnvironment
    {
        private readonly IImageApi _imageApi;
        private readonly IContainerApi _containerApi;
        private readonly ILogger? _logger;

        public string Name { get; init; }

        public Container[] Containers { get; init; }

#pragma warning disable SA1201 // Elements should appear in the correct order
        public DockerEnvironment(string name, Container[] containers)
#pragma warning restore SA1201 // Elements should appear in the correct order
            : this(name, containers, CreateDefaultDockerClient())
        {
        }

        public DockerEnvironment(string name, Container[] containers, ILogger logger)
            : this(name, containers, new ImageApi(logger), new ContainerApi(logger), logger)
        {
        }

        public DockerEnvironment(string name, Container[] containers, IDockerClient dockerClient)
            : this(name, containers, new ImageApi(dockerClient), new ContainerApi(dockerClient), null)
        {
        }

        public DockerEnvironment(string name, Container[] containers, IDockerClient dockerClient, ILogger? logger)
            : this(name, containers, new ImageApi(dockerClient), new ContainerApi(dockerClient), logger)
        {
        }

        public DockerEnvironment(string name, Container[] containers, IImageApi imageApi, IContainerApi containerApi, ILogger? logger) =>
            (Name, Containers, _imageApi, _containerApi, _logger) = (name, containers, imageApi, containerApi, logger);

        public async Task UpAsync(CancellationToken cancellationToken = default)
        {
            // Pull/build all required images.
            var ensureTasks = Containers.Select(c => c.EnsureImageAvailableAsync(cancellationToken));
            await Task.WhenAll(ensureTasks);

            // Run all containers.
            var runTasks = Containers.Select(c => c.RunAsync(cancellationToken));
            await Task.WhenAll(runTasks);
        }

        public async Task DownAsync(CancellationToken cancellationToken = default)
        {
            var stopTasks = Containers.Select(c => c.StopAsync(cancellationToken));
            await Task.WhenAll(stopTasks);
        }

        public Container? GetContainer(string name) =>
            GetContainer<Container>(name);

        public TContainer? GetContainer<TContainer>(string name)
            where TContainer : Container
        {
            var containerName = GetContainerName(Name, name);
            return Containers.FirstOrDefault(container => container is TContainer c && c.Name == containerName) as TContainer;
        }

        public async ValueTask DisposeAsync()
        {
            var disposeTasks = Containers.Select(c => c.DisposeAsync());
            foreach (var disposeTask in disposeTasks)
            {
                await disposeTask;
            }
        }
    }
}
