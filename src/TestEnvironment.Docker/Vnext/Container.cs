using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestEnvironment.Docker.Vnext
{
    public class Container : IDisposable, IAsyncDisposable
    {
        public void Dispose()
        {
            new ContainerFromDockerfileBuilder().SetName("").SetDockerfile();
            throw new NotImplementedException();
        }

        public ValueTask DisposeAsync()
        {
            throw new NotImplementedException();
        }
    }
}
