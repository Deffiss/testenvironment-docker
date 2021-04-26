using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TestEnvironment.Docker.Vnext.ContainerLifecycle;
using TestEnvironment.Docker.Vnext.ContainerOperations;
using TestEnvironment.Docker.Vnext.ImageOperations;

namespace TestEnvironment.Docker.Vnext
{
    public class Container : IAsyncDisposable
    {
        private readonly IContainerApi _containerApi;
        private readonly IImageApi _imageApi;
        private readonly

        public string Name { get; init; }

        public string ImageName { get; init; }

        public string Tag { get; init; }

        public IDictionary<string, string> EnvironmentVariables { get; init; }

        public IDictionary<ushort, ushort> Ports { get; private set; }

        public bool Reusable { get; init; } = false;

        public IList<string>? Entrypoint { get; init; }

        public IList<ushort>? ExposedPorts { get; init; }

        public IContainerInitializer? ContainerInitializer { get; init; }

        public IContainerWaiter? ContainerWaiter { get; init; }

        public IContainerCleaner? ContainerCleaner { get; init; }

        public string? Id { get; private set; }

        public string? IPAddress { get; private set; }

#pragma warning disable SA1201 // Elements should appear in the correct order
        public Container(ContainerParameters containerParameters, IContainerApi containerApi, ImageApi imageApi)
        {
#pragma warning restore SA1201 // Elements should appear in the correct order
            (Name, ImageName, Tag, EnvironmentVariables, Ports, Reusable, Entrypoint, ExposedPorts,
            ContainerInitializer, ContainerWaiter, ContainerCleaner) = containerParameters;
            (_containerApi, _imageApi) = (containerApi, imageApi);
        }

        public async Task RunAsync(CancellationToken cancellationToken = default)
        {
            var runtimeInfo = _containerApi.RunContainerAsync()

            if (waiter is not null)
            {
                var isStarted = await waiter.WaitAsync(container, cancellationToken);
                if (!isStarted)
                {
                    _logger.LogError($"Container {name} didn't start.");
                    throw new TimeoutException($"Container {name} didn't start.");
                }
            }

            if (initializer is not null)
            {
                await initializer.InitializeAsync(container, cancellationToken);
            }

            if (reusable && cleaner is not null)
            {
                await cleaner.CleanupAsync(container, cancellationToken);
            }
            throw new NotImplementedException();
        }

        public Task StopAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async virtual Task EnsureImageAvailableAsync(CancellationToken cancellationToken = default) =>
            await _imageApi.PullImageAsync(ImageName, Tag, cancellationToken);

        public ValueTask DisposeAsync()
        {
            throw new NotImplementedException();
        }

        internal void SetRuntimeInfo(string? id, string? ipAddress, IDictionary<ushort, ushort> ports) =>
            (Id, IPAddress, Ports) = (id, ipAddress, ports);
    }
}
