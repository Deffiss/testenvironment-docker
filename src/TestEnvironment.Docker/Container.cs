using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet;
using Microsoft.Extensions.Logging;
using TestEnvironment.Docker.ContainerLifecycle;
using TestEnvironment.Docker.ContainerOperations;
using TestEnvironment.Docker.ImageOperations;
using static TestEnvironment.Docker.DockerClientExtentions;

namespace TestEnvironment.Docker
{
    public class Container : IAsyncDisposable
    {
        private readonly ContainerParameters _containerParameters;

        public string Name => _containerParameters.Name;

        public string ImageName => _containerParameters.ImageName;

        public string Tag => _containerParameters.Tag;

        public IDictionary<string, string>? EnvironmentVariables => _containerParameters.EnvironmentVariables;

        public IDictionary<ushort, ushort>? Ports { get; private set; }

        public bool Reusable => _containerParameters.Reusable;

        public IList<string>? Entrypoint => _containerParameters.Entrypoint;

        public IList<ushort>? ExposedPorts => _containerParameters.ExposedPorts;

        public bool IsDockerInDocker => _containerParameters.IsDockerInDocker;

        public IContainerInitializer? ContainerInitializer => _containerParameters.ContainerInitializer;

        public IContainerWaiter? ContainerWaiter => _containerParameters.ContainerWaiter;

        public IContainerCleaner? ContainerCleaner => _containerParameters.ContainerCleaner;

        public string? Id { get; private set; }

        public string? IPAddress { get; private set; }

        protected IContainerApi ContainerApi { get; init; }

        protected IImageApi ImageApi { get; init; }

        protected ILogger? Logger { get; init; }

#pragma warning disable SA1201 // Elements should appear in the correct order
        public Container(ContainerParameters containerParameters)
            : this(containerParameters, CreateDefaultDockerClient())
        {
        }

        public Container(ContainerParameters containerParameters, IDockerClient dockerClient)
            : this(containerParameters, new ContainerApi(dockerClient), new ImageApi(dockerClient), null)
        {
        }

        public Container(ContainerParameters containerParameters, IDockerClient dockerClient, ILogger? logger)
            : this(containerParameters, new ContainerApi(dockerClient, logger), new ImageApi(dockerClient, logger), logger)
        {
        }

        public Container(ContainerParameters containerParameters, IContainerApi containerApi, ImageApi imageApi, ILogger? logger) =>
#pragma warning restore SA1201 // Elements should appear in the correct order
            (_containerParameters, ContainerApi, ImageApi, Logger, Ports) =
            (containerParameters, containerApi, imageApi, logger, containerParameters.Ports);

        public async Task RunAsync(CancellationToken cancellationToken = default)
        {
            var runtimeInfo = await ContainerApi.RunContainerAsync(_containerParameters, cancellationToken);
            (Id, IPAddress, Ports) = runtimeInfo;

            if (ContainerWaiter is not null)
            {
                var isStarted = await ContainerWaiter.WaitAsync(this, cancellationToken);
                if (!isStarted)
                {
                    Logger?.LogError($"Container {Name} didn't start.");
                    throw new TimeoutException($"Container {Name} didn't start.");
                }
            }

            if (ContainerInitializer is not null)
            {
                await ContainerInitializer.InitializeAsync(this, cancellationToken);
            }

            if (Reusable && ContainerCleaner is not null)
            {
                await ContainerCleaner.CleanupAsync(this, cancellationToken);
            }
        }

        public async Task<string> ExecAsync(string[] cmd, IDictionary<string, string>? env = null, string? user = null, string workdir = "/", CancellationToken cancellationToken = default)
        {
            if (Id is null)
            {
                throw new InvalidOperationException("Container is not run.");
            }

            return await ContainerApi.ExecAsync(Id, cmd, env, user, workdir, cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            if (Id is null)
            {
                throw new InvalidOperationException("Container is not run.");
            }

            await ContainerApi.StopContainerAsync(Id, cancellationToken);
        }

        public async virtual Task EnsureImageAvailableAsync(CancellationToken cancellationToken = default) =>
            await ImageApi.PullImageAsync(ImageName, Tag, cancellationToken);

        public async ValueTask DisposeAsync()
        {
            if (Id is not null)
            {
                await ContainerApi.RemoveContainerAsync(Id);
            }
        }
    }
}
