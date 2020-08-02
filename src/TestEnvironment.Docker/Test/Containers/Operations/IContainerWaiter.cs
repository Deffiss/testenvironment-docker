using System.Threading;
using System.Threading.Tasks;

namespace TestEnvironment.Docker.Test.Containers.Operations
{
    public interface IContainerWaiter
    {
        Task<bool> Wait<TContainer, TConfiguration>(TContainer container, CancellationToken cancellationToken = default)
            where TContainer : Container<TConfiguration>
            where TConfiguration : ContainerConfiguration;
    }
}
