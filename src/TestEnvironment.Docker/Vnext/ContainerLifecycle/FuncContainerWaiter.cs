using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace TestEnvironment.Docker.Vnext.ContainerLifecycle
{
    public class FuncContainerWaiter : BaseContainerWaiter<Container>
    {
        private readonly Func<Container, CancellationToken, Task<bool>> _waitFunc;

        public FuncContainerWaiter(Func<Container, CancellationToken, Task<bool>> waitFunc) =>
            _waitFunc = waitFunc;

        public FuncContainerWaiter(Func<Container, CancellationToken, Task<bool>> waitFunc, ILogger logger)
            : base(logger)
        {
            _waitFunc = waitFunc;
        }

        protected override Task<bool> PerformCheckAsync(Container container, CancellationToken cancellationToken) =>
            _waitFunc(container, cancellationToken);
    }
}
