using System.Threading;
using System.Threading.Tasks;

namespace TestEnvironment.Docker.Vnext.ContainerLifecycle
{
    public interface IContainerWaiter
    {
        Task<bool> WaitAsync(Container container, CancellationToken cancellationToken = default);
    }

    public interface IContainerWaiter<in TContainer> : IContainerWaiter
        where TContainer : Container
    {
        Task<bool> WaitAsync(TContainer container, CancellationToken cancellationToken = default);
    }
}
