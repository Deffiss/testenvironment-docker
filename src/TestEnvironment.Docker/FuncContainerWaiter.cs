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

        public async Task<(bool IsReady, string DebugMessage)> Wait(Container container, CancellationToken cancellationToken = default) => (await _waitFunc(container), (string)null);
    }
}
