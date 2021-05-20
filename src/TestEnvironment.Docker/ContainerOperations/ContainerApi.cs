using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Extensions.Logging;
using static TestEnvironment.Docker.DockerClientExtentions;

namespace TestEnvironment.Docker.ContainerOperations
{
    public class ContainerApi : IContainerApi
    {
        private readonly IDockerClient _dockerClient;
        private readonly ILogger? _logger;

        public ContainerApi()
            : this(CreateDefaultDockerClient(), null)
        {
        }

        public ContainerApi(IDockerClient dockerClient)
            : this(dockerClient, null)
        {
        }

        public ContainerApi(ILogger logger)
            : this(CreateDefaultDockerClient(), logger)
        {
        }

        public ContainerApi(IDockerClient dockerClient, ILogger? logger)
        {
            _dockerClient = dockerClient;
            _logger = logger;
        }

        public async Task<ContainerRuntimeInfo> RunContainerAsync(ContainerParameters containerParameters, CancellationToken cancellationToken = default)
        {
            var (name, reusable) = (containerParameters.Name, containerParameters.Reusable);

            // Try to find container in docker session
            var startedContainer = await GetContainerAsync(name, cancellationToken);

            // If container already exist - remove that
            if (startedContainer != null)
            {
                // TODO: check status and network
                if (!reusable)
                {
                    await _dockerClient.Containers.RemoveContainerAsync(
                        startedContainer.ID,
                        new ContainerRemoveParameters { Force = true },
                        cancellationToken);

                    startedContainer = await CreateContainer(containerParameters, cancellationToken);
                }
            }
            else
            {
                startedContainer = await CreateContainer(containerParameters, cancellationToken);
            }

            _logger?.LogInformation($"Container '{name}' has been run.");
            _logger?.LogDebug($"Container state: {startedContainer.State}");
            _logger?.LogDebug($"Container status: {startedContainer.Status}");
            _logger?.LogDebug(
                $"Container IPAddress: {startedContainer.NetworkSettings.Networks.FirstOrDefault().Key} - {startedContainer.NetworkSettings.Networks.FirstOrDefault().Value.IPAddress}");

            var ipAddress = startedContainer.NetworkSettings.Networks.FirstOrDefault().Value.IPAddress;
            var ports = startedContainer.Ports.DistinctBy(p => p.PrivatePort).ToDictionary(p => p.PrivatePort, p => p.PublicPort);

            return new(startedContainer.ID, ipAddress, ports);
        }

        public async Task StopContainerAsync(string id, CancellationToken cancellationToken = default) =>
            await _dockerClient.Containers.StopContainerAsync(id, new ContainerStopParameters { }, cancellationToken);

        public async Task RemoveContainerAsync(string id, CancellationToken cancellationToken = default) =>
            await _dockerClient.Containers.RemoveContainerAsync(id, new ContainerRemoveParameters { Force = true });

        private async Task<ContainerListResponse> CreateContainer(ContainerParameters containerParameters, CancellationToken cancellationToken)
        {
            // Create new container
            var createParams = GetCreateContainerParameters(containerParameters);

            var containerInstance = await _dockerClient.Containers.CreateContainerAsync(createParams, cancellationToken);

            // Run container
            await _dockerClient.Containers.StartContainerAsync(containerInstance.ID, new ContainerStartParameters(), cancellationToken);

            // Try to find container in docker session
#pragma warning disable CS8603 // Possible null reference return.
            return await GetContainerAsync(containerParameters.Name, cancellationToken);
#pragma warning restore CS8603 // Possible null reference return.
        }

        private async Task<ContainerListResponse?> GetContainerAsync(string name, CancellationToken cancellationToken)
        {
            var containerName = $"/{name}";

            var containers = await _dockerClient.Containers.ListContainersAsync(
                new ContainersListParameters
                {
                    All = true,
                    Filters = new Dictionary<string, IDictionary<string, bool>>
                    {
                        ["name"] = new Dictionary<string, bool>
                        {
                            [containerName] = true
                        }
                    }
                },
                cancellationToken);

            return containers?.FirstOrDefault(x => x.Names.Contains(containerName));
        }

        private CreateContainerParameters GetCreateContainerParameters(ContainerParameters containerParameters)
        {
            var (name, imageName, tag, environmentVariables, ports, entrypoint, exposedPorts) =
                (containerParameters.Name, containerParameters.ImageName, containerParameters.Tag, containerParameters.EnvironmentVariables, containerParameters.Ports, containerParameters.Entrypoint, containerParameters.ExposedPorts);

            var stringifiedVariables = environmentVariables?.Select(p => $"{p.Key}={p.Value}")?.ToArray()
                ?? Array.Empty<string>();

            var createParams = new CreateContainerParameters
            {
                Name = name,
                Image = $"{imageName}:{tag}",
                AttachStdout = true,
                Env = stringifiedVariables,
                Hostname = name,
                HostConfig = new HostConfig
                {
                    PublishAllPorts = ports == null
                }
            };

            if (ports is not null)
            {
                createParams.HostConfig.PortBindings = ports
                    .ToDictionary(
                        p => $"{p.Key}/tcp",
                        p => (IList<PortBinding>)new List<PortBinding>
                            { new PortBinding { HostPort = p.Value.ToString() } });
            }

            if (entrypoint is not null && entrypoint.Any())
            {
                createParams.Entrypoint = entrypoint;
            }

            if (exposedPorts is not null && exposedPorts.Any())
            {
                createParams.ExposedPorts = exposedPorts.ToDictionary(p => $"{p}/tcp", p => default(EmptyStruct));
            }

            return createParams;
        }
    }
}
