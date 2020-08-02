using System.Threading;
using System.Threading.Tasks;

namespace TestEnvironment.Docker.Test.Containers.Operations
{
    public interface IContainerWaiter<TContainer>
        where TContainer : Container
    {
        Task<bool> Wait(TContainer container, CancellationToken cancellationToken = default);
    }
}
