using System.Threading;
using System.Threading.Tasks;

namespace TestEnvironment.Docker.Test.Containers.Operations
{
    public interface IContainerInitializer
    {
        Task<bool> Initialize<TContainer, TConfiguration>(TContainer container, CancellationToken cancellationToken)
            where TContainer : Container<TConfiguration>
            where TConfiguration : ContainerConfiguration;
    }
}
