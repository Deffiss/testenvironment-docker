using System.Threading;
using System.Threading.Tasks;
using TestEnvironment.Docker.DockerApi.Models;
using TestEnvironment.Docker.Test.Containers;

namespace TestEnvironment.Docker.DockerApi.Services
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
