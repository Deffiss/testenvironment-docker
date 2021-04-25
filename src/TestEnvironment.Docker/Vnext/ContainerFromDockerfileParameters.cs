using System.Collections.Generic;

namespace TestEnvironment.Docker.Vnext
{
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
    public record ContainerFromDockerfileParameters(string Name, string Dockerfile)
        : ContainerParameters(Name, Name)
#pragma warning restore SA1313 // Parameter names should begin with lower-case letter
    {
        public IDictionary<string, string> BuildArgs { get; init; } = new Dictionary<string, string>();

        public string Context { get; init; } = ".";
    }
}
