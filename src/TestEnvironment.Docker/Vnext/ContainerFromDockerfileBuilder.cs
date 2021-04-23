using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestEnvironment.Docker.Vnext
{
    public sealed class ContainerFromDockerfileBuilder : ContainerBuilder
    {
        private ContainerFromDockerfileParameters _containerFromDockerfileParameters = new ContainerFromDockerfileParameters("a", "b");

        protected override ContainerParameters ContainerParameters
        {
            get => _containerFromDockerfileParameters;
            set => _containerFromDockerfileParameters = (ContainerFromDockerfileParameters)value;
        }

        public override ContainerFromDockerfileBuilder SetName(string name)
        {
            base.SetName(name);
            return this;
        }

        public ContainerFromDockerfileBuilder SetDockerfile() => this;
    }
}
