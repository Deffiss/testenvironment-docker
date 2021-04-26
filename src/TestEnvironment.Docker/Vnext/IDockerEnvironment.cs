using System;
using System.Threading;
using System.Threading.Tasks;

namespace TestEnvironment.Docker.Vnext
{
    public interface IDockerEnvironment : IAsyncDisposable
    {
        Task Up(CancellationToken cancellationToken = default);

        Task Down(CancellationToken cancellationToken = default);
    }
}
