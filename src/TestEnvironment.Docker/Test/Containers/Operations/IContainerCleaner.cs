using System.Threading;
using System.Threading.Tasks;

namespace TestEnvironment.Docker.Test.Containers.Operations
{
    public interface IContainerCleaner<TContainer>
        where TContainer : Container
    {
        Task CleanUp(TContainer container, CancellationToken cancellationToken = default);
    }
}
