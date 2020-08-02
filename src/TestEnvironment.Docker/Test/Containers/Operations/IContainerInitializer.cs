using System.Threading;
using System.Threading.Tasks;

namespace TestEnvironment.Docker.Test.Containers.Operations
{
    public interface IContainerInitializer<TContainer>
        where TContainer : Container
    {
        Task<bool> Initialize(TContainer container, CancellationToken cancellationToken);
    }
}
