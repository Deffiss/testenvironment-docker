using System.Threading;
using System.Threading.Tasks;

namespace TestEnvironment.Docker.Vnext.ContainerLifecycle
{
    public interface IContainerCleaner
    {
        Task CleanupAsync(Container container, CancellationToken token = default);
    }

    public interface IContainerCleaner<TContainer> : IContainerCleaner
    {
        Task CleanupAsync(TContainer container, CancellationToken token = default);
    }
}
