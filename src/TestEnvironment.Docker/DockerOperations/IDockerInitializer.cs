using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace TestEnvironment.Docker.DockerOperations
{
    public interface IDockerInitializer : IAsyncDisposable
    {
        Task InitializeDockerAsync(CancellationToken cancellationToken = default);
    }
}
