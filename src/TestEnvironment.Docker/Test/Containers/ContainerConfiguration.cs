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

        public Operations.IContainerWaiter ContainerWaiter { get; set; }

        public Operations.IContainerCleaner ContainerCleaner { get; set; }

        public Operations.IContainerInitializer ContainerInitializer { get; set; }
    }
}
