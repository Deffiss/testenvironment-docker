using System.Collections.Generic;

namespace TestEnvironment.Docker.DockerApi.Abstractions.Models
{
    public class ContainerConfiguration
    {
        public ContainerConfiguration(string name, string imageName, string tag, IDictionary<string, string> environmentVariables, IDictionary<ushort, ushort> ports)
        {
            Name = name;
            ImageName = imageName;
            Tag = tag;
            EnvironmentVariables = environmentVariables;
            Ports = ports;
        }

        public string Name { get; }

        public string ImageName { get; }

        public string Tag { get; }

        public IDictionary<string, string> EnvironmentVariables { get; }

        public IDictionary<ushort, ushort> Ports { get; }
    }
}
