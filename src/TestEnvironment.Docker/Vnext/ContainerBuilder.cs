using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestEnvironment.Docker.Vnext
{
    public class ContainerBuilder
    {
        protected virtual ContainerParameters ContainerParameters { get; set; } = new ContainerParameters("a", "b");

        public virtual ContainerBuilder SetName(string name)
        {
            ContainerParameters = ContainerParameters with
            {
                Name = name
            };

            return this;
        }
    }
}
