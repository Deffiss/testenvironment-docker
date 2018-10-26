using System.Threading;
using System.Threading.Tasks;

namespace TestEnvironment.Docker
{
    public interface IContainerWaiter
    {
        Task<(bool IsReady, string DebugMessage)> Wait(Container container, CancellationToken cancellationToken);
    }

    public interface IContainerWaiter<TContainer> : IContainerWaiter
    {
        Task<(bool IsReady, string DebugMessage)> Wait(TContainer container, CancellationToken cancellationToken);
    }
}
