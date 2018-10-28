using System;
using System.Threading;
using System.Threading.Tasks;

namespace TestEnvironment.Docker
{
    public class FuncContainerWaiter : IContainerWaiter
    {
        private readonly Func<Container, Task<bool>> _waitFunc;

        public FuncContainerWaiter(Func<Container, Task<bool>> waitFunc)
        {
            _waitFunc = waitFunc;
        }

        public Task<bool> Wait(Container container, CancellationToken cancellationToken = default) => _waitFunc(container);
    }
}
