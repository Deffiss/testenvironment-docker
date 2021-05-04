using Docker.DotNet;
using Microsoft.Extensions.Logging;
using TestEnvironment.Docker.Vnext.ContainerOperations;
using TestEnvironment.Docker.Vnext.ImageOperations;

namespace TestEnvironment.Docker.Vnext
{
    public class ContainerFromDockerfile : Container
    {
        public ContainerFromDockerfile(ContainerParameters containerParameters)
            : base(containerParameters)
        {
        }

        public ContainerFromDockerfile(ContainerParameters containerParameters, IDockerClient dockerClient)
            : base(containerParameters, dockerClient)
        {
        }

        public ContainerFromDockerfile(ContainerParameters containerParameters, IDockerClient dockerClient, ILogger? logger)
            : base(containerParameters, dockerClient, logger)
        {
        }

        public ContainerFromDockerfile(ContainerParameters containerParameters, IContainerApi containerApi, ImageApi imageApi, ILogger? logger)
            : base(containerParameters, containerApi, imageApi, logger)
        {
        }
    }
}
