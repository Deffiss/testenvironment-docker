using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TestEnvironment.Docker.ContainerOperations
{
    public interface IContainerApi
    {
        Task<ContainerRuntimeInfo> RunContainerAsync(ContainerParameters containerParameters, CancellationToken cancellationToken = default);

        Task<string> ExecAsync(string containerId, string[] cmd, IDictionary<string, string>? env = null, string? user = null, string workdir = "/", CancellationToken cancellationToken = default);

        Task StopContainerAsync(string id, CancellationToken cancellationToken = default);

        Task RemoveContainerAsync(string id, CancellationToken cancellationToken = default);
    }
}
