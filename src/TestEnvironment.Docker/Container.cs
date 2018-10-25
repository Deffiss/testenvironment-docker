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
    public class Container : IDependency
    {
        protected ILogger Logger { get; }

        protected bool IsDockerInDocker { get; }

        protected DockerClient DockerClient { get; }

        public string Name { get; }

        public string Id { get; private set; }

        public string IPAddress { get; private set; }

        public Dictionary<ushort, ushort> Ports { get; private set; }

        public string ImageName { get; }

        public string Tag { get; }

        public IDictionary<string, string> EnvironmentVariables { get; }

        public Container(DockerClient dockerClient, string name, string imageName, string tag = "latest", IDictionary<string, string> environmentVariables = null, bool isDockerInDocker = false, ILogger logger = null)
        {
            Name = name;
            DockerClient = dockerClient;
            Logger = logger;
            IsDockerInDocker = isDockerInDocker;
            ImageName = imageName ?? throw new ArgumentNullException(nameof(imageName));
            Tag = tag;
            EnvironmentVariables = environmentVariables ?? new Dictionary<string, string>();
        }

        public async Task Run(IDictionary<string, string> environmentVariables, CancellationToken token = default)
        {
            if (environmentVariables == null) throw new ArgumentNullException(nameof(environmentVariables));

            // Make sure that we don't try to add the same var twice.
            var nonExistentEnvironmentVariables = environmentVariables.Where(e => !EnvironmentVariables.ContainsKey(e.Key));

            var mergedVariables = EnvironmentVariables.Concat(nonExistentEnvironmentVariables);
            var stringifiedVariables = mergedVariables.Select(p => $"{p.Key}={p.Value}").ToArray();

            await RunContainerSafely(stringifiedVariables, token);

            await WaitForReadiness(token);
        }

        public Task Stop(CancellationToken token = default) =>
            DockerClient.Containers.StopContainerAsync(Id, new ContainerStopParameters { });

        protected virtual Task WaitForReadiness(CancellationToken token = default) => Task.CompletedTask;

        private async Task RunContainerSafely(string[] environmentVariables, CancellationToken token)
        {
            // Try to find container in docker session
            var containers = await DockerClient.Containers.ListContainersAsync(
                new ContainersListParameters
                {
                    All = true,
                    Filters = new Dictionary<string, IDictionary<string, bool>> { ["name"] = new Dictionary<string, bool> { [$"/{Name}"] = true } }
                }, token);

            var startedContainer = containers.FirstOrDefault();

            // If container already exist - remove that
            if (startedContainer != null)
            {
                await DockerClient.Containers.RemoveContainerAsync(startedContainer.ID, new ContainerRemoveParameters { Force = true }, token);
            }

            // Create new container
            var container = await DockerClient.Containers.CreateContainerAsync(
                new CreateContainerParameters
                {
                    Name = Name,
                    Image = $"{ImageName}:{Tag}",
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
            containers = await DockerClient.Containers.ListContainersAsync(
                new ContainersListParameters
                {
                    All = true,
                    Filters = new Dictionary<string, IDictionary<string, bool>> { ["name"] = new Dictionary<string, bool> { [$"/{Name}"] = true } }
                }, token);

            startedContainer = containers.First();

            Logger.LogInformation($"Container '{Name}' has been run.");
            Logger.LogDebug($"Container state: {startedContainer.State}");
            Logger.LogDebug($"Container status: {startedContainer.Status}");
            Logger.LogDebug($"Container IPAddress: {startedContainer.NetworkSettings.Networks.FirstOrDefault().Key} - {startedContainer.NetworkSettings.Networks.FirstOrDefault().Value.IPAddress}");

            Id = container.ID;
            IPAddress = startedContainer.NetworkSettings.Networks.FirstOrDefault().Value.IPAddress;
            Ports = startedContainer.Ports.ToDictionary(p => p.PrivatePort, p => p.PublicPort);
        }

        public void Dispose()
        {
            if (!string.IsNullOrEmpty(Id))
            {
                DockerClient.Containers.RemoveContainerAsync(Id, new ContainerRemoveParameters { Force = true }).Wait();
            }
        }
    }
}
