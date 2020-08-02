using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using TestEnvironment.Docker.DockerApi.Abstractions.Models;
using TestEnvironment.Docker.DockerApi.Abstractions.Services;

namespace TestEnvironment.Docker.DockerApi.Internal.Services
{
    public class DockerContainersService : IDockerContainersService
    {
        private readonly DockerClient _client;

        public DockerContainersService(DockerClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<string> CreateContainer(ContainerConfiguration configuration, CancellationToken cancellationToken = default)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var parameters = configuration.ToCreateContainerParameters();
            var createdContainer = await _client.Containers.CreateContainerAsync(parameters, cancellationToken);

            return createdContainer.ID;
        }

        public async Task<ContainerInfo> GetContainerByName(string name, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Container Name shouldn't be null or empty or whitespace!", nameof(name));
            }

            var containers = await _client.Containers.ListContainersAsync(
                new ContainersListParameters
                {
                    All = true,
                    Filters = new Dictionary<string, IDictionary<string, bool>> { ["name"] = new Dictionary<string, bool> { [$"/{name}"] = true } }
                }, cancellationToken);

            var startedContainer = containers.FirstOrDefault();

            return startedContainer?.ToContainerInfo();
        }

        public Task RemoveContainerById(string id, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Container Id shouldn't be null or empty or whitespace!", nameof(id));
            }

            return _client.Containers.RemoveContainerAsync(id, new ContainerRemoveParameters { Force = true }, cancellationToken);
        }

        public Task<bool> StartContainer(string id, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Container Id shouldn't be null or empty or whitespace!", nameof(id));
            }

            return _client.Containers.StartContainerAsync(id, new ContainerStartParameters(), cancellationToken);
        }

        public Task<bool> StopContainer(string id, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Container Id shouldn't be null or empty or whitespace!", nameof(id));
            }

            return _client.Containers.StopContainerAsync(id, new ContainerStopParameters(), cancellationToken);
        }
    }
}
