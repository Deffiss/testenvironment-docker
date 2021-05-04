using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestEnvironment.Docker.Vnext.ContainerLifecycle
{
    public abstract class BaseContainerWaiter<TContainer> : IContainerWaiter<TContainer>
        where TContainer : Container
    {
        public Task<bool> WaitAsync(TContainer container, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> WaitAsync(Container container, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
