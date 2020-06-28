using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet;
using Microsoft.Extensions.Logging;
using TestEnvironment.Docker.DockerApi.Abstractions.Models;
using TestEnvironment.Docker.DockerApi.Abstractions.Services;
using TestEnvironment.Docker.DockerApi.Internal.Services;

namespace TestEnvironment.Docker
{
    public class Container : IDependency
    {
        private readonly IContainerWaiter _containerWaiter;
        private readonly IContainerCleaner _containerCleaner;
        private readonly bool _reuseContainer;
        private readonly IDockerContainersService _dockerContainersService;

        public Container(
            DockerClient dockerClient,
            string name,
            string imageName,
            string tag = "latest",
            IDictionary<string, string> environmentVariables = null,
            IDictionary<ushort, ushort> ports = null,
            bool isDockerInDocker = false,
            bool reuseContainer = false,
            IContainerWaiter containerWaiter = null,
            IContainerCleaner containerCleaner = null,
            ILogger logger = null)
        {
            Name = name;
            Logger = logger;
            IsDockerInDocker = isDockerInDocker;
            ImageName = imageName ?? throw new ArgumentNullException(nameof(imageName));
            Tag = tag;
            EnvironmentVariables = environmentVariables ?? new Dictionary<string, string>();
            Ports = ports;
            _containerWaiter = containerWaiter;
            _containerCleaner = containerCleaner;
            _reuseContainer = reuseContainer;
            _dockerContainersService = new DockerContainersService(dockerClient);
        }

        public bool IsDockerInDocker { get; }

        public string Name { get; }

        public string Id { get; private set; }

        public string IPAddress { get; private set; }

        public IDictionary<ushort, ushort> Ports { get; private set; }

        public string ImageName { get; }

        public string Tag { get; }

        public IDictionary<string, string> EnvironmentVariables { get; }

        protected ILogger Logger { get; }

        public async Task Run(IDictionary<string, string> environmentVariables, CancellationToken token = default)
        {
            if (environmentVariables == null)
            {
                throw new ArgumentNullException(nameof(environmentVariables));
            }

            // Make sure that we don't try to add the same var twice.
            await RunContainerSafely(EnvironmentVariables.MergeDictionaries(environmentVariables), token);

            if (_containerWaiter != null)
            {
                var isStarted = await _containerWaiter.Wait(this, token);
                if (!isStarted)
                {
                    Logger.LogError($"Container {Name} didn't start.");
                    throw new TimeoutException($"Container {Name} didn't start.");
                }
            }

            if (_reuseContainer && _containerCleaner != null)
            {
                await _containerCleaner.Cleanup(this, token);
            }
        }

        public Task Stop(CancellationToken token = default) => _dockerContainersService.StopContainer(Id, token);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsync(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!string.IsNullOrEmpty(Id))
                {
                    _dockerContainersService.RemoveContainerById(Id, default).Wait();
                }
            }
        }

        protected async virtual ValueTask DisposeAsync(bool disposing)
        {
            if (disposing)
            {
                if (!string.IsNullOrEmpty(Id))
                {
                    await _dockerContainersService.RemoveContainerById(Id, default);
                }
            }
        }

        private async Task RunContainerSafely(IDictionary<string, string> environmentVariables, CancellationToken token = default)
        {
            // Try to find container in docker session
            var runningContainer = await _dockerContainersService.GetContainerByName(Name, token);
            var configuration = new ContainerConfiguration(Name, ImageName, Tag, environmentVariables, Ports);

            // If container already exist - remove that
            if (runningContainer != null)
            {
                // TODO: check status and network
                if (!_reuseContainer)
                {
                    await _dockerContainersService.RemoveContainerById(runningContainer.Id, token);
                    runningContainer = await CreateContainer(configuration, token);
                }
            }
            else
            {
                runningContainer = await CreateContainer(configuration, token);
            }

            Logger.LogInformation($"Container '{Name}' has been run.");
            Logger.LogDebug($"Container state: {runningContainer.State}");
            Logger.LogDebug($"Container status: {runningContainer.Status}");
            Logger.LogDebug($"Container IPAddress: {runningContainer.NetworkName} - {runningContainer.IPAddress}");

            Id = runningContainer.Id;
            IPAddress = runningContainer.IPAddress;
            Ports = runningContainer.Ports;
        }

        private async Task<ContainerInfo> CreateContainer(ContainerConfiguration configuration, CancellationToken token)
        {
            var createdContainerId = await _dockerContainersService.CreateContainer(configuration, token);

            // Run container
            await _dockerContainersService.StartContainer(createdContainerId, token);

            // Try to find container in docker session
            var startedContainer = await _dockerContainersService.GetContainerByName(Name, token);

            return startedContainer;
        }
    }
}
