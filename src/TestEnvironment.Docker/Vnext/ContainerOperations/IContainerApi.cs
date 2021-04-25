using System.Threading;
using System.Threading.Tasks;

namespace TestEnvironment.Docker.Vnext.ContainerOperations
{
    public interface IContainerApi
    {
        Task RunContainerAsync(Container container, CancellationToken token = default);

        Task StopContainerAsync(string id, CancellationToken token = default);
    }
}
