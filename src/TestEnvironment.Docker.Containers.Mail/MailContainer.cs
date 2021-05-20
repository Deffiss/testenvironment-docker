using System.Collections.Generic;
using Docker.DotNet;
using Microsoft.Extensions.Logging;
using TestEnvironment.Docker.ContainerOperations;
using TestEnvironment.Docker.ImageOperations;

namespace TestEnvironment.Docker.Containers.Mail
{
    public class MailContainer : Container
    {
        public MailContainer(MailContainerParameters containerParameters)
            : base(containerParameters)
        {
        }

        public MailContainer(MailContainerParameters containerParameters, IDockerClient dockerClient)
            : base(containerParameters, dockerClient)
        {
        }

        public MailContainer(MailContainerParameters containerParameters, IDockerClient dockerClient, ILogger? logger)
            : base(containerParameters, dockerClient, logger)
        {
        }

        public MailContainer(MailContainerParameters containerParameters, IContainerApi containerApi, ImageApi imageApi, ILogger? logger)
            : base(containerParameters, containerApi, imageApi, logger)
        {
        }
    }
}
