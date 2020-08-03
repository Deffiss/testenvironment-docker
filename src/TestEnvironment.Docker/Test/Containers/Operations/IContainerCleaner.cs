using System.Threading;
using System.Threading.Tasks;

namespace TestEnvironment.Docker.Test.Containers.Operations
{
    public interface IContainerCleaner
    {
        Task CleanUp<TContainer>(TContainer container, CancellationToken cancellationToken = default)
            where TContainer : Container;
    }
}
