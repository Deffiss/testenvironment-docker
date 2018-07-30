using Docker.DotNet;
using Docker.DotNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TestEnvironment.Docker
{
    public class Container : IDependency
    {
        private readonly string _imageName;
        private readonly string _tag;
        private readonly IDictionary<string, string> _environmentVariables;

        protected Action<string> Logger { get; }

        protected bool IsDockerInDocker { get; }

        protected DockerClient DockerClient { get; }

        public string Name { get; }

        public string Id { get; private set; }

        public string IPAddress { get; private set; }

        public Dictionary<ushort, ushort> Ports { get; private set; }

        public Container(DockerClient dockerClient, string name, string imageName, string tag = "latest", (string Name, string Value)[] environmentVariables = null, Action<string> logger = null, bool isDockerInDocker = false)
        {
            Name = name;
            DockerClient = dockerClient;
            Logger = logger;
            IsDockerInDocker = isDockerInDocker;
            _imageName = imageName ?? throw new ArgumentNullException(nameof(imageName));
            _tag = tag;
            _environmentVariables = environmentVariables?.ToDictionary(e => e.Name, e => e.Value) ?? new Dictionary<string, string>();
        }

        public async Task Run((string Name, string Value)[] environmentVariables, CancellationToken token = default)
        {
            if (environmentVariables == null) throw new ArgumentNullException(nameof(environmentVariables));

            var mergedVariables = _environmentVariables.Concat(environmentVariables.ToDictionary(e => e.Name, e => e.Value));
            var stringifiedVariables = mergedVariables.Select(p => $"{p.Key}={p.Value}").ToArray();

            await RunContainerSafely(stringifiedVariables, token);

            await WaitForReadiness(token);
        }

        public Task Stop(CancellationToken token = default) =>
            DockerClient.Containers.StopContainerAsync(Id, new ContainerStopParameters { });

        protected virtual Task WaitForReadiness(CancellationToken token = default) => Task.CompletedTask;

        private async Task RunContainerSafely(string[] environmentVariables, CancellationToken token)
        {
            // Create container name
            Logger?.Invoke($"Container name: {Name}");

            // Try to find container in docker session
            var containers = await DockerClient.Containers.ListContainersAsync(new ContainersListParameters { All = true }, token);

            var startedContainer = containers.FirstOrDefault(c => c.Names.Contains($"/{Name}"));

            // If container already exist - remove that
            if (startedContainer != null)
            {
                await DockerClient.Containers.RemoveContainerAsync(startedContainer.ID, new ContainerRemoveParameters { Force = true }, token);
            }

            var images = await DockerClient.Images.ListImagesAsync(new ImagesListParameters
            {
                All = true,
                MatchName = $"{_imageName}:{_tag}"
            }, token);

            // If image not pulled yet - pull this.
            if (!images.Any())
            {
                await DockerClient.Images.CreateImageAsync(
                    new ImagesCreateParameters
                    {
                        FromImage = _imageName,
                        Tag = _tag
                    },
                    null,
                    new Progress<JSONMessage>(m => Logger?.Invoke($"Pulling image {_imageName}:{_tag}:\n{m.ProgressMessage}")),
                    token);
            }

            // Create new container
            var container = await DockerClient.Containers.CreateContainerAsync(
                new CreateContainerParameters
                {
                    Name = Name,
                    Image = $"{_imageName}:{_tag}",
                    AttachStdout = true,
                    Env = environmentVariables,
                    Hostname = Name,
                    Domainname = Name,
                    HostConfig = new HostConfig
                    {
                        PublishAllPorts = true,
                    },
                }, token);

            // Run container
            await DockerClient.Containers.StartContainerAsync(container.ID, new ContainerStartParameters(), token);

            // Try to find container in docker session
            containers = await DockerClient.Containers.ListContainersAsync(new ContainersListParameters { All = true }, token);

            startedContainer = containers.FirstOrDefault(c => c.Names.Contains($"/{Name}"));

            Logger?.Invoke($"Container state: {startedContainer?.State}");
            Logger?.Invoke($"Container status: {startedContainer?.Status}");
            Logger?.Invoke($"Container IPAddress: {startedContainer?.NetworkSettings.Networks.FirstOrDefault().Key} - {startedContainer?.NetworkSettings.Networks.FirstOrDefault().Value.IPAddress}");

            Id = container.ID;
            IPAddress = startedContainer?.NetworkSettings.Networks.FirstOrDefault().Value.IPAddress;
            Ports = startedContainer?.Ports.ToDictionary(p => p.PrivatePort, p => p.PublicPort);
        }

        public void Dispose()
        {
            if (!string.IsNullOrEmpty(Id))
            {
                DockerClient.Containers.RemoveContainerAsync(Id, new ContainerRemoveParameters { Force = true }).Wait();
            }
        }

        public Task Cleanup(CancellationToken token = default) => throw new NotImplementedException();
    }
}
