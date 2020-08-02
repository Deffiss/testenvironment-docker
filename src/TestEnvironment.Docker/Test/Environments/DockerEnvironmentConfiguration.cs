using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace TestEnvironment.Docker.Test.Environments
{
    public class DockerEnvironmentConfiguration
    {
        public string EnvironmentName { get; set; }

        public IDictionary<string, string> EnvironmentVariables { get; set; }

        public bool IsDockerInDocker { get; set; }

        public ILogger Logger { get; set; }

        public string[] IgnoredFolders { get; set; }

        public IList<Containers.Container> Containers { get; set; }
    }
}
