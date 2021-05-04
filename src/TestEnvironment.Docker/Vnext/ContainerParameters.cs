using System.Collections.Generic;
using TestEnvironment.Docker.Vnext.ContainerLifecycle;

namespace TestEnvironment.Docker.Vnext
{
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
    public record ContainerParameters(string Name, string ImageName)
#pragma warning restore SA1313 // Parameter names should begin with lower-case letter
    {
        public string Tag { get; init; } = "latest";

        public IDictionary<string, string>? EnvironmentVariables { get; init; }

        public IDictionary<ushort, ushort>? Ports { get; init; }

        public bool Reusable { get; init; } = false;

        public bool IsDockerInDocker { get; init; } = false;

        public IList<string>? Entrypoint { get; init; }

        public IList<ushort>? ExposedPorts { get; init; }

        public IContainerInitializer? ContainerInitializer { get; init; }

        public IContainerWaiter? ContainerWaiter { get; init; }

        public IContainerCleaner? ContainerCleaner { get; init; }

        public void Deconstruct(out IContainerInitializer? containerInitializer, out IContainerWaiter? containerWaiter, out IContainerCleaner? containerCleaner) =>
            (containerInitializer, containerWaiter, containerCleaner) = (ContainerInitializer, ContainerWaiter, ContainerCleaner);

#pragma warning disable SA1117 // Parameters should be on same line or separate lines
        public void Deconstruct(out string name, out string imageName, out string tag, out IDictionary<string, string>? environmentVariables,
            out IDictionary<ushort, ushort>? ports, out bool reusable, out bool isDockerInDocker, out IList<string>? entrypoint, out IList<ushort>? exposedPorts,
            out IContainerInitializer? containerInitializer, out IContainerWaiter? containerWaiter, out IContainerCleaner? containerCleaner) =>
            (name, imageName, tag, environmentVariables, ports, reusable, isDockerInDocker, entrypoint, exposedPorts, containerInitializer, containerWaiter, containerCleaner) =
            (Name, ImageName, Tag, EnvironmentVariables, Ports, Reusable, IsDockerInDocker, Entrypoint, ExposedPorts, ContainerInitializer, ContainerWaiter, ContainerCleaner);
#pragma warning restore SA1117 // Parameters should be on same line or separate lines
    }
}
