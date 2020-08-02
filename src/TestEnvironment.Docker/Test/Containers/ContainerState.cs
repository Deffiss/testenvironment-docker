using System.Collections.Generic;

namespace TestEnvironment.Docker.Test.Containers
{
    public class ContainerState
    {
        public ContainerState(string id, string ipAddress, IDictionary<ushort, ushort> ports)
        {
            Id = id;
            IPAddress = ipAddress;
            Ports = ports;
        }

        public string Id { get; }

        public string IPAddress { get; }

        public IDictionary<ushort, ushort> Ports { get; }
    }
}
