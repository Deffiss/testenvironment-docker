using System.Threading;
using System.Threading.Tasks;

namespace TestEnvironment.Docker.Vnext.ContainerOperations
{
    public interface IContainerApi
    {
        Task<ContainerRuntimeInfo> RunContainerAsync(ContainerParameters containerParameters, CancellationToken token = default);

        Task StopContainerAsync(string id, CancellationToken token = default);
    }
}
