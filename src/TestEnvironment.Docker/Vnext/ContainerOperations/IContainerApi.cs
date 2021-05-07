using System.Threading;
using System.Threading.Tasks;

namespace TestEnvironment.Docker.Vnext.ContainerOperations
{
    public interface IContainerApi
    {
        Task<ContainerRuntimeInfo> RunContainerAsync(ContainerParameters containerParameters, CancellationToken cancellationToken = default);

        Task StopContainerAsync(string id, CancellationToken cancellationToken = default);

        Task RemoveContainerAsync(string id, CancellationToken cancellationToken = default);
    }
}
