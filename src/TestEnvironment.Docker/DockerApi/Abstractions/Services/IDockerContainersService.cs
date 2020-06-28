using System.Threading;
using System.Threading.Tasks;
using TestEnvironment.Docker.DockerApi.Abstractions.Models;

namespace TestEnvironment.Docker.DockerApi.Abstractions.Services
{
    public interface IDockerContainersService
    {
        Task<string> CreateContainer(ContainerConfiguration configuration, CancellationToken cancellationToken);

        Task<ContainerInfo> GetContainerByName(string name, CancellationToken cancellationToken);

        Task RemoveContainerById(string id, CancellationToken cancellationToken);

        Task<bool> StartContainer(string id, CancellationToken cancellationToken);

        Task<bool> StopContainer(string id, CancellationToken cancellationToken);
    }
}
