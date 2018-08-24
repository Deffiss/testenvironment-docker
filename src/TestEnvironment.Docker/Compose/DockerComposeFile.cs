using SharpYaml.Serialization;
using System.Collections.Generic;

namespace TestEnvironment.Docker.Compose
{
    public class DockerComposeFile
    {
        [YamlMember("version")]
        public string Version { get; set; }

        [YamlMember("services")]
        public IDictionary<string, Service> Services { get; set; }

        public class Service
        {
            [YamlMember("container_name")]
            public string ContainerName { get; set; }

            [YamlMember("image")]
            public string Image { get; set; }

            [YamlMember("ports")]
            public List<string> Ports { get; set; }

            [YamlMember("environment")]
            public IDictionary<string, string> Environment { get; set; }
        }
    }
}
