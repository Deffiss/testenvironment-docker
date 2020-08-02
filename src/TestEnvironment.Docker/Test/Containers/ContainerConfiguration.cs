using System.Collections.Generic;

namespace TestEnvironment.Docker.Test.Containers
{
    public class ContainerConfiguration
    {
        public string Name { get; set; }

        public string ImageName { get; set; }

        public string Tag { get; set; }

        public bool ReuseContainer { get; set; }

        public IList<string> EntryPoint { get; set; }

        public IList<string> ExposedPorts { get; set; }

        public IDictionary<string, string> EnvironmentVariables { get; set; }

        public IDictionary<ushort, ushort> Ports { get; set; }

        public Operations.IContainerWaiter<Container> ContainerWaiter { get; set; }

        public Operations.IContainerCleaner<Container> ContainerCleaner { get; set; }

        public Operations.IContainerInitializer<Container> ContainerInitializer { get; set; }
    }
}
