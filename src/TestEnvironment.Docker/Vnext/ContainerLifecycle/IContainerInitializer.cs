using System.Threading;
using System.Threading.Tasks;

namespace TestEnvironment.Docker.Vnext.ContainerLifecycle
{
    public interface IContainerInitializer
    {
        Task<bool> InitializeAsync(Container container, CancellationToken cancellationToken = default);
    }

    public interface IContainerInitializer<in TContainer> : IContainerInitializer
        where TContainer : Container
    {
        Task<bool> InitializeAsync(TContainer container, CancellationToken cancellationToken = default);
    }
}
