using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TestEnvironment.Docker.Vnext
{
    public class Container : IDisposable, IAsyncDisposable
    {
        public string? Id { get; private set; }

        public string? IPAddress { get; private set; }

        public IDictionary<ushort, ushort>? Ports { get; private set; }

        public ContainerParameters Parameters { get; init; }

#pragma warning disable SA1201 // Elements should appear in the correct order
        public Container(ContainerParameters containerParameters)
#pragma warning restore SA1201 // Elements should appear in the correct order
        {
            Parameters = containerParameters;
        }

        public Task RunAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task StopAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public ValueTask DisposeAsync()
        {
            throw new NotImplementedException();
        }

        internal void SetRuntimeInfo(string? id, string? ipAddress, IDictionary<ushort, ushort> ports) =>
            (Id, IPAddress, Ports) = (id, ipAddress, ports);
    }
}
