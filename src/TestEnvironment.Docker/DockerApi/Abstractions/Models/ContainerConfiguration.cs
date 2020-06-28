using System.Collections.Generic;

namespace TestEnvironment.Docker.DockerApi.Abstractions.Models
{
    public class ContainerConfiguration
    {
        public ContainerConfiguration(
            string name,
            string imageName,
            string tag,
            IList<string> entryPoint,
            IList<string> exposedPorts,
            IDictionary<string, string> environmentVariables,
            IDictionary<ushort, ushort> ports)
        {
            Name = name;
            ImageName = imageName;
            Tag = tag;
            EntryPoint = entryPoint;
            ExposedPorts = exposedPorts;
            EnvironmentVariables = environmentVariables;
            Ports = ports;
        }

        public string Name { get; }

        public string ImageName { get; }

        public string Tag { get; }

        public IList<string> EntryPoint { get; }

        public IList<string> ExposedPorts { get; set; }

        public IDictionary<string, string> EnvironmentVariables { get; }

        public IDictionary<ushort, ushort> Ports { get; }
    }
}
