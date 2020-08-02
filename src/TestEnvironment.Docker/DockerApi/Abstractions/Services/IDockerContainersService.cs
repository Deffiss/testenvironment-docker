using System.Threading;
using System.Threading.Tasks;
using TestEnvironment.Docker.DockerApi.Abstractions.Models;

namespace TestEnvironment.Docker.DockerApi.Abstractions.Services
{
    public interface IDockerContainersService
    {
        Task<string> CreateContainer(ContainerConfiguration configuration, CancellationToken cancellationToken = default);

        Task<ContainerInfo> GetContainerByName(string name, CancellationToken cancellationToken = default);

        Task RemoveContainerById(string id, CancellationToken cancellationToken = default);

        Task<bool> StartContainer(string id, CancellationToken cancellationToken = default);

        Task<bool> StopContainer(string id, CancellationToken cancellationToken = default);
    }
}
