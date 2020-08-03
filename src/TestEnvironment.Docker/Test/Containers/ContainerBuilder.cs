using System.Collections.Generic;

namespace TestEnvironment.Docker.Test.Containers
{
    public class ContainerBuilder
    {
        public ContainerBuilder()
        {
            Configuration = new ContainerConfiguration();
        }

        protected ContainerConfiguration Configuration { get; }

        public ContainerBuilder SetName(string name)
        {
            Configuration.Name = name;

            return this;
        }

        public ContainerBuilder SetImageName(string imageName)
        {
            Configuration.ImageName = imageName;

            return this;
        }

        public ContainerBuilder SetTag(string tag)
        {
            Configuration.Tag = tag;

            return this;
        }

        public ContainerBuilder SetReuseContainer(bool reuseContainer)
        {
            Configuration.ReuseContainer = reuseContainer;

            return this;
        }

        public ContainerBuilder SetEntryPoint(IList<string> entryPoint)
        {
            Configuration.EntryPoint = entryPoint;

            return this;
        }

        public ContainerBuilder SetExposedPorts(IList<string> exposedPorts)
        {
            Configuration.ExposedPorts = exposedPorts;

            return this;
        }

        public ContainerBuilder SetEnvironmentVariables(IDictionary<string, string> environmentVariables)
        {
            Configuration.EnvironmentVariables = environmentVariables;

            return this;
        }

        public ContainerBuilder SetPorts(IDictionary<ushort, ushort> ports)
        {
            Configuration.Ports = ports;

            return this;
        }

        public ContainerBuilder SetContainerWaiter(Operations.IContainerWaiter containerWaiter)
        {
            Configuration.ContainerWaiter = containerWaiter;

            return this;
        }

        public ContainerBuilder SetContainerCleaner(Operations.IContainerCleaner containerCleaner)
        {
            Configuration.ContainerCleaner = containerCleaner;

            return this;
        }

        public ContainerBuilder SetContainerInitializer(Operations.IContainerInitializer containerInitializer)
        {
            Configuration.ContainerInitializer = containerInitializer;

            return this;
        }

        public Container Build()
        {
            return new Container(Configuration);
        }
    }
}
