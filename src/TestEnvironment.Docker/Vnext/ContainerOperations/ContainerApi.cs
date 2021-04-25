using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Extensions.Logging;
using static TestEnvironment.Docker.Vnext.DockerClientExtentions;

namespace TestEnvironment.Docker.Vnext.ContainerOperations
{
    public class ContainerApi : IContainerApi
    {
        private readonly DockerClient _dockerClient;
        private readonly ILogger? _logger;

        public ContainerApi()
            : this(CreateDefaultDockerClient(), null)
        {
        }

        public ContainerApi(ILogger logger)
            : this(CreateDefaultDockerClient(), logger)
        {
        }

        public ContainerApi(DockerClient dockerClient, ILogger? logger)
        {
            _dockerClient = dockerClient;
            _logger = logger;
        }

        public async Task RunContainerAsync(Container container, CancellationToken cancellationToken = default)
        {
            var parameters = container.Parameters;

            await RunContainerSafely(container, cancellationToken);

            var (initializer, waiter, cleaner) = parameters;

            if (waiter is not null)
            {
                var isStarted = await waiter.WaitAsync(container, cancellationToken);
                if (!isStarted)
                {
                    _logger.LogError($"Container {parameters.Name} didn't start.");
                    throw new TimeoutException($"Container {parameters.Name} didn't start.");
                }
            }

            if (initializer is not null)
            {
                await initializer.InitializeAsync(container, cancellationToken);
            }

            if (parameters.Reusable && cleaner is not null)
            {
                await cleaner.CleanupAsync(container, cancellationToken);
            }
        }

        public Task StopContainerAsync(string id, CancellationToken token = default)
        {
            throw new System.NotImplementedException();
        }

        private async Task RunContainerSafely(Container container, CancellationToken cancellationToken)
        {
            var parameters = container.Parameters;

            // Try to find container in docker session
            var startedContainer = await GetContainerAsync(parameters.Name, cancellationToken);

            // If container already exist - remove that
            if (startedContainer != null)
            {
                // TODO: check status and network
                if (!parameters.Reusable)
                {
                    await _dockerClient.Containers.RemoveContainerAsync(
                        startedContainer.ID,
                        new ContainerRemoveParameters { Force = true },
                        cancellationToken);

                    startedContainer = await CreateContainer(stringifiedVariables, cancellationToken);
                }
            }
            else
            {
                startedContainer = await CreateContainer(stringifiedVariables, cancellationToken);
            }

            _logger?.LogInformation($"Container '{parameters.Name}' has been run.");
            _logger?.LogDebug($"Container state: {startedContainer.State}");
            _logger?.LogDebug($"Container status: {startedContainer.Status}");
            _logger?.LogDebug(
                $"Container IPAddress: {startedContainer.NetworkSettings.Networks.FirstOrDefault().Key} - {startedContainer.NetworkSettings.Networks.FirstOrDefault().Value.IPAddress}");

            var ipAddress = startedContainer.NetworkSettings.Networks.FirstOrDefault().Value.IPAddress;
            var ports = startedContainer.Ports.ToDictionary(p => p.PrivatePort, p => p.PublicPort);
            container.SetRuntimeInfo(startedContainer.ID, ipAddress, ports);
        }

        private async Task<ContainerListResponse> CreateContainer(Container container, CancellationToken cancellationToken)
        {
            // Create new container
            var createParams = GetCreateContainerParameters(container);

            var containerInstance = await _dockerClient.Containers.CreateContainerAsync(createParams, cancellationToken);

            // Run container
            await _dockerClient.Containers.StartContainerAsync(containerInstance.ID, new ContainerStartParameters(), cancellationToken);

            // Try to find container in docker session
            return await GetContainerAsync(container.Parameters.Name, cancellationToken);
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
                }, cancellationToken);

            return containers?.FirstOrDefault(x => x.Names.Contains(containerName));
        }

        private CreateContainerParameters GetCreateContainerParameters(Container container)
        {
            var parameters = container.Parameters;

            // Make sure that we don't try to add the same var twice.
            var stringifiedVariables = parameters.EnvironmentVariables
                .Select(p => $"{p.Key}={p.Value}").ToArray();

            var createParams = new CreateContainerParameters
            {
                Name = parameters.Name,
                Image = $"{parameters.ImageName}:{parameters.Tag}",
                AttachStdout = true,
                Env = stringifiedVariables,
                Hostname = parameters.Name,
                HostConfig = new HostConfig
                {
                    PublishAllPorts = parameters.Ports == null
                }
            };

            if (parameters.Ports is not null)
            {
                createParams.HostConfig.PortBindings = parameters.Ports
                    .ToDictionary(
                        p => $"{p.Key}/tcp",
                        p => (IList<PortBinding>)new List<PortBinding>
                            { new PortBinding { HostPort = p.Value.ToString() } });
            }

            if (parameters.Entrypoint is not null && parameters.Entrypoint.Any())
            {
                createParams.Entrypoint = parameters.Entrypoint;
            }

            // Exposed ports

            return createParams;
        }
    }
}
