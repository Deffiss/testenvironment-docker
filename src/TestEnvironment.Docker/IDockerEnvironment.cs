using System;
using System.Threading;
using System.Threading.Tasks;

namespace TestEnvironment.Docker
{
    public interface IDockerEnvironment : IAsyncDisposable
    {
        string Name { get; }

        Container[] Containers { get; }

        Task UpAsync(CancellationToken cancellationToken = default);

        Task DownAsync(CancellationToken cancellationToken = default);

        Container? GetContainer(string name);

        TContainer? GetContainer<TContainer>(string name)
            where TContainer : Container;
    }
}
