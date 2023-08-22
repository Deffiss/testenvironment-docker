using System.Collections.Generic;
using System.Reflection;
using TestEnvironment.Docker.ContainerLifecycle;

namespace TestEnvironment.Docker
{
    public record ContainerParameters(string Name, string ImageName)
    {
        public string Tag { get; init; } = "latest";

        public IDictionary<string, string>? EnvironmentVariables { get; init; }

        public IDictionary<ushort, ushort>? Ports { get; init; }

        public bool Reusable { get; init; } = false;

        public bool IsDockerInDocker { get; init; } = false;

        public IList<string>? Entrypoint { get; init; }

        public IList<ushort>? ExposedPorts { get; init; }

        public IList<string>? Binds { get; init; }

        public IContainerInitializer? ContainerInitializer { get; init; }

        public IContainerWaiter? ContainerWaiter { get; init; }

        public IContainerCleaner? ContainerCleaner { get; init; }

        public void Deconstruct(out IContainerInitializer? containerInitializer, out IContainerWaiter? containerWaiter, out IContainerCleaner? containerCleaner) =>
            (containerInitializer, containerWaiter, containerCleaner) = (ContainerInitializer, ContainerWaiter, ContainerCleaner);
    }
}
