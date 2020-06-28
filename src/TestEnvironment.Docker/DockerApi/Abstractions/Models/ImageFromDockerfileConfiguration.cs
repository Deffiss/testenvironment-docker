using System.Collections.Generic;

namespace TestEnvironment.Docker.DockerApi.Abstractions.Models
{
    public class ImageFromDockerfileConfiguration
    {
        public ImageFromDockerfileConfiguration(string imageName, string tag, string dockerfile, IDictionary<string, string> buildArgs)
        {
            ImageName = imageName;
            Tag = tag;
            Dockerfile = dockerfile;
            BuildArgs = buildArgs;
        }

        public string ImageName { get; }

        public string Tag { get; }

        public string Dockerfile { get; }

        public IDictionary<string, string> BuildArgs { get; }
    }
}
