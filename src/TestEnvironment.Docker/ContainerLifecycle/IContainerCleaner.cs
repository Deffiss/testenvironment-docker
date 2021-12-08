using System.Threading;
using System.Threading.Tasks;

namespace TestEnvironment.Docker.ContainerLifecycle
{
    public interface IContainerCleaner
    {
        Task CleanupAsync(Container container, CancellationToken cancellationToken = default);
    }

    public interface IContainerCleaner<in TContainer> : IContainerCleaner
        where TContainer : Container
    {
        Task CleanupAsync(TContainer container, CancellationToken cancellationToken = default);
    }
}
