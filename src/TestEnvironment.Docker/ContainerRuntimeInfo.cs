using System.Collections.Generic;

namespace TestEnvironment.Docker
{
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
    public record ContainerRuntimeInfo(string Id, string IPAddress, IDictionary<ushort, ushort> Ports)
#pragma warning restore SA1313 // Parameter names should begin with lower-case letter
    {
    }
}
