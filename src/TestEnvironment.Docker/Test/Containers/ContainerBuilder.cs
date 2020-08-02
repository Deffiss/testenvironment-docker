using System.Collections.Generic;

namespace TestEnvironment.Docker.Test.Containers
{
    public class ContainerBuilder : BaseContainerBuilder<Container, ContainerConfiguration>
    {
        private readonly ContainerConfiguration _configuration;

        public ContainerBuilder()
        {
            _configuration = new ContainerConfiguration();
        }

        public ContainerBuilder SetName(string name)
        {
            _configuration.Name = name;

            return this;
        }

        public ContainerBuilder SetImageName(string imageName)
        {
            _configuration.ImageName = imageName;

            return this;
        }

        public ContainerBuilder SetTag(string tag)
        {
            _configuration.Tag = tag;

            return this;
        }

        public ContainerBuilder SetReuseContainer(bool reuseContainer)
        {
            _configuration.ReuseContainer = reuseContainer;

            return this;
        }

        public ContainerBuilder SetEntryPoint(IList<string> entryPoint)
        {
            _configuration.EntryPoint = entryPoint;

            return this;
        }

        public ContainerBuilder SetExposedPorts(IList<string> exposedPorts)
        {
            _configuration.ExposedPorts = exposedPorts;

            return this;
        }

        public ContainerBuilder SetEnvironmentVariables(IDictionary<string, string> environmentVariables)
        {
            _configuration.EnvironmentVariables = environmentVariables;

            return this;
        }

        public ContainerBuilder SetPorts(IDictionary<ushort, ushort> ports)
        {
            _configuration.Ports = ports;

            return this;
        }

        public ContainerBuilder SetContainerWaiter(Operations.IContainerWaiter<Container> containerWaiter)
        {
            _configuration.ContainerWaiter = containerWaiter;

            return this;
        }

        public ContainerBuilder SetContainerCleaner(Operations.IContainerCleaner<Container> containerCleaner)
        {
            _configuration.ContainerCleaner = containerCleaner;

            return this;
        }

        public ContainerBuilder SetContainerInitializer(Operations.IContainerInitializer<Container> containerInitializer)
        {
            _configuration.ContainerInitializer = containerInitializer;

            return this;
        }

        public override Container Build()
        {
            return new Container(_configuration);
        }
    }
}
