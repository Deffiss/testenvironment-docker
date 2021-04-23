using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestEnvironment.Docker.Vnext
{
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
    public record ContainerParameters(string Name, string ImageName)
#pragma warning restore SA1313 // Parameter names should begin with lower-case letter
    {
        public string Tag { get; init; } = "latest";

        public IDictionary<string, string> EnvironmentVariables { get; init; } = new Dictionary<string, string>();

        public IDictionary<ushort, ushort> Ports { get; set; } = new Dictionary<ushort, ushort>();

        public bool Reusable { get; init; } = false;

        public IContainerInitializer? ContainerInitializer { get; init; }

        public IContainerWaiter? ContainerWaiter { get; init; }

        public IContainerCleaner? ContainerCleaner { get; init; }

        public IList<string>? Entrypoint { get; init; }
    }
}
