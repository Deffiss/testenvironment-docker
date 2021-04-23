using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestEnvironment.Docker.Vnext
{
    public interface IContainerOperatopns
    {
        Task RunContainer(Container container, CancellationToken token = default);

        Task StopContainer(Container container, CancellationToken token = default);
    }
}
