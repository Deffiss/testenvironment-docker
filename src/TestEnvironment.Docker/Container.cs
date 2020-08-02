using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet;
using Microsoft.Extensions.Logging;
using TestEnvironment.Docker.DockerApi.Models;
using TestEnvironment.Docker.DockerApi.Services;
using TestEnvironment.Docker.Test.Helpers;

namespace TestEnvironment.Docker
{
    public class Container : IDependency
    {
        private readonly IContainerWaiter _containerWaiter;
        private readonly IContainerInitializer _containerInitializer;
        private readonly IContainerCleaner _containerCleaner;
        private readonly IDockerContainersService _dockerContainersService;
        private readonly bool _reuseContainer;

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
            ILogger logger = null,
            IList<string> entrypoint = null,
            IContainerInitializer containerInitializer = null)
        {
            Name = name;
            Logger = logger;
            IsDockerInDocker = isDockerInDocker;
            ImageName = imageName ?? throw new ArgumentNullException(nameof(imageName));
            Tag = tag;
            EnvironmentVariables = environmentVariables ?? new Dictionary<string, string>();
            Ports = ports;
            _containerWaiter = containerWaiter;
            _containerInitializer = containerInitializer;
            _containerCleaner = containerCleaner;
            _reuseContainer = reuseContainer;

            Entrypoint = entrypoint;
            _dockerContainersService = new DockerContainersService(dockerClient);
        }

        public bool IsDockerInDocker { get; }

        public string Name { get; }

        public string Id { get; private set; }

        public string IPAddress { get; private set; }

        public IDictionary<ushort, ushort> Ports { get; private set; }

        public IList<string> Entrypoint { get; }

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

            if (_containerInitializer != null)
            {
                await _containerInitializer.Initialize(this, token);
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

        protected virtual async ValueTask DisposeAsync(bool disposing)
        {
            if (disposing)
            {
                if (!string.IsNullOrEmpty(Id))
                {
                    await _dockerContainersService.RemoveContainerById(Id, default);
                }
            }
        }

        protected virtual ContainerConfiguration GetContainerConfiguration(IDictionary<string, string> environmentVariables, IList<string> exposedPorts) => new ContainerConfiguration(
            Name,
            ImageName,
            Tag,
            Entrypoint,
            exposedPorts,
            environmentVariables,
            Ports);

        private async Task RunContainerSafely(IDictionary<string, string> environmentVariables, CancellationToken token = default)
        {
            // Try to find container in docker session
            var runningContainer = await _dockerContainersService.GetContainerByName(Name, token);
            var configuration = GetContainerConfiguration(environmentVariables, null);

            // If container already exist - remove that
            // TODO: check status and network
            if (runningContainer != null && !_reuseContainer)
            {
                await _dockerContainersService.RemoveContainerById(runningContainer.Id, token);
                runningContainer = await CreateContainer(configuration, token);
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
