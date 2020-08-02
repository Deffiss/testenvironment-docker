using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TestEnvironment.Docker.DockerApi.Models;
using TestEnvironment.Docker.DockerApi.Services;
using TestEnvironment.Docker.Test.Helpers;

namespace TestEnvironment.Docker.Test.Containers
{
    public class Container<TConfiguration>
        where TConfiguration : ContainerConfiguration
    {
        private readonly IDockerContainersService _dockerContainersService;
        private ContainerState _containerState;
        private Environments.DockerEnvironment _currentDockerEnvironment;

        public Container(TConfiguration configuration)
        {
            Configuration = configuration;
            _dockerContainersService = new DockerContainersService(DockerClientStorage.DockerClient);
        }

        public string Name => Configuration.Name;

        public string ImageName => Configuration.ImageName;

        public string Tag => Configuration.Tag;

        // TODO - refactor it
        public bool IsDockerInDocker => _currentDockerEnvironment?.IsDockerInDocker ?? true;

        public string Id => _containerState?.Id;

        public string IPAddress => _containerState?.IPAddress;

        public IDictionary<ushort, ushort> Ports => _containerState?.Ports;

        protected TConfiguration Configuration { get; }

        public static ContainerBuilder<Container<ContainerConfiguration>, ContainerConfiguration> Create()
        {
            return new ContainerBuilder<Container<ContainerConfiguration>, ContainerConfiguration>();
        }

        public void AssignDockerEnvironment(Environments.DockerEnvironment dockerEnvironment)
        {
            _currentDockerEnvironment = dockerEnvironment;
        }

        public async Task<ContainerInfo> Run(IDictionary<string, string> environmentVariables, CancellationToken token = default)
        {
            var containerInfo = await RunContainerSafely(Configuration.EnvironmentVariables.MergeDictionaries(environmentVariables), token);

            if (Configuration.ContainerWaiter != null)
            {
                var isStarted = await Configuration.ContainerWaiter.Wait(this, token);
                if (!isStarted)
                {
                    throw new TimeoutException($"Container {Configuration.Name} didn't start.");
                }
            }

            if (Configuration.ReuseContainer && Configuration.ContainerCleaner != null)
            {
                await Configuration.ContainerCleaner.CleanUp(this, token);
            }

            if (Configuration.ContainerInitializer != null)
            {
                await Configuration.ContainerInitializer.Initialize(this, token);
            }

            return containerInfo;
        }

        public Task Stop(CancellationToken token = default)
        {
            return _dockerContainersService.StopContainer(Id, token);
        }

        private async Task<ContainerInfo> RunContainerSafely(IDictionary<string, string> environmentVariables, CancellationToken token = default)
        {
            // Try to find container in docker session
            var runningContainer = await _dockerContainersService.GetContainerByName(Configuration.Name, token);

            // If container already exist - remove that
            if (runningContainer != null && !Configuration.ReuseContainer)
            {
                await _dockerContainersService.RemoveContainerById(runningContainer.Id, token);
                runningContainer = await CreateContainer(Configuration, token);
            }
            else
            {
                runningContainer = await CreateContainer(Configuration, token);
            }

            _containerState = new ContainerState(runningContainer.Id, runningContainer.IPAddress, runningContainer.Ports);

            return runningContainer;
        }

        private async Task<ContainerInfo> CreateContainer(ContainerConfiguration configuration, CancellationToken token)
        {
            var createdContainerId = await _dockerContainersService.CreateContainer(configuration, token);

            // Run container
            await _dockerContainersService.StartContainer(createdContainerId, token);

            // Try to find container in docker session
            var startedContainer = await _dockerContainersService.GetContainerByName(Configuration.Name, token);

            return startedContainer;
        }
    }
}
