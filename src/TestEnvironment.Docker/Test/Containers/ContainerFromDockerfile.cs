using System.Collections.Generic;

namespace TestEnvironment.Docker.Test.Containers
{
    public class ContainerFromDockerfile : Container
    {
        public ContainerFromDockerfile(string dockerfile, IDictionary<string, string> buildArgs, string context, ContainerConfiguration configuration)
            : base(configuration)
        {
            Dockerfile = dockerfile;
            BuildArgs = buildArgs;
            Context = context;
        }

        public string Dockerfile { get; }

        public IDictionary<string, string> BuildArgs { get; }

        public string Context { get; }
    }
}
