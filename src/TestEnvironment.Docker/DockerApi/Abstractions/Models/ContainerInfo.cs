using System.Collections.Generic;

namespace TestEnvironment.Docker.DockerApi.Abstractions.Models
{
    public class ContainerInfo
    {
        public ContainerInfo(string id, string state, string status, string networkName, string ipAddress, IDictionary<ushort, ushort> ports)
        {
            Id = id;
            State = state;
            Status = status;
            NetworkName = networkName;
            IPAddress = ipAddress;
            Ports = ports;
        }

        public string Id { get; }

        public string State { get; }

        public string Status { get; }

        public string NetworkName { get; }

        public string IPAddress { get; }

        public IDictionary<ushort, ushort> Ports { get; }
    }
}