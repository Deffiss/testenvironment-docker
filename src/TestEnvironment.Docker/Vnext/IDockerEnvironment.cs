using System;
using System.Threading;
using System.Threading.Tasks;

namespace TestEnvironment.Docker.Vnext
{
    public interface IDockerEnvironment : IAsyncDisposable
    {
        string Name { get; }

        Container[] Containers { get; }

        Task UpAsync(CancellationToken cancellationToken = default);

        Task DownAsync(CancellationToken cancellationToken = default);

        Container? GetContainer<TContainer>(string name)
            where TContainer : Container;
    }
}
