using System.Collections.Generic;

namespace TestEnvironment.Docker.Test.Containers
{
    public class ContainerBuilder<TContainer, TConfiguration> : BaseContainerBuilder<Container<TConfiguration>, ContainerConfiguration>
        where TContainer : Container<TConfiguration>
        where TConfiguration : ContainerConfiguration, new()
    {
        public ContainerBuilder()
        {
            Configuration = new TConfiguration();
        }

        protected TConfiguration Configuration { get; }

        public ContainerBuilder<TContainer, TConfiguration> SetName(string name)
        {
            Configuration.Name = name;

            return this;
        }

        public ContainerBuilder<TContainer, TConfiguration> SetImageName(string imageName)
        {
            Configuration.ImageName = imageName;

            return this;
        }

        public ContainerBuilder<TContainer, TConfiguration> SetTag(string tag)
        {
            Configuration.Tag = tag;

            return this;
        }

        public ContainerBuilder<TContainer, TConfiguration> SetReuseContainer(bool reuseContainer)
        {
            Configuration.ReuseContainer = reuseContainer;

            return this;
        }

        public ContainerBuilder<TContainer, TConfiguration> SetEntryPoint(IList<string> entryPoint)
        {
            Configuration.EntryPoint = entryPoint;

            return this;
        }

        public ContainerBuilder<TContainer, TConfiguration> SetExposedPorts(IList<string> exposedPorts)
        {
            Configuration.ExposedPorts = exposedPorts;

            return this;
        }

        public ContainerBuilder<TContainer, TConfiguration> SetEnvironmentVariables(IDictionary<string, string> environmentVariables)
        {
            Configuration.EnvironmentVariables = environmentVariables;

            return this;
        }

        public ContainerBuilder<TContainer, TConfiguration> SetPorts(IDictionary<ushort, ushort> ports)
        {
            Configuration.Ports = ports;

            return this;
        }

        public ContainerBuilder<TContainer, TConfiguration> SetContainerWaiter(Operations.IContainerWaiter containerWaiter)
        {
            Configuration.ContainerWaiter = containerWaiter;

            return this;
        }

        public ContainerBuilder<TContainer, TConfiguration> SetContainerCleaner(Operations.IContainerCleaner containerCleaner)
        {
            Configuration.ContainerCleaner = containerCleaner;

            return this;
        }

        public ContainerBuilder<TContainer, TConfiguration> SetContainerInitializer(Operations.IContainerInitializer containerInitializer)
        {
            Configuration.ContainerInitializer = containerInitializer;

            return this;
        }

        public override Container<TConfiguration> Build()
        {
            return new Container<TConfiguration>(Configuration);
        }
    }
}
