using System.Collections.Generic;

namespace TestEnvironment.Docker
{
    public record ContainerFromDockerfileParameters(string Name, string Dockerfile)
        : ContainerParameters(Name, Name)
    {
        public IDictionary<string, string>? BuildArgs { get; init; } = new Dictionary<string, string>();

        public string Context { get; init; } = ".";

        public string[]? IgnoredContextFiles { get; init; }
    }
}
