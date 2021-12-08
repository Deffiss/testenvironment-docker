using System.Collections.Generic;
using Docker.DotNet;
using Microsoft.Extensions.Logging;
using TestEnvironment.Docker.ContainerOperations;
using TestEnvironment.Docker.ImageOperations;
using IP = System.Net.IPAddress;

namespace TestEnvironment.Docker.Containers.Ftp
{
    public class FtpContainer : Container
    {
        private readonly FtpContainerParameters _parameters;

        public string FtpUserName => _parameters.FtpUserName;

        public string FtpPassword => _parameters.FtpPassword;

        public string? FtpHost => IsDockerInDocker ? IPAddress : IP.Loopback.ToString();

#pragma warning disable SA1201 // Elements should appear in the correct order
        public FtpContainer(FtpContainerParameters containerParameters)
#pragma warning restore SA1201 // Elements should appear in the correct order
            : base(containerParameters) =>
            _parameters = containerParameters;

        public FtpContainer(FtpContainerParameters containerParameters, IDockerClient dockerClient)
            : base(containerParameters, dockerClient) =>
            _parameters = containerParameters;

        public FtpContainer(FtpContainerParameters containerParameters, IDockerClient dockerClient, ILogger? logger)
            : base(containerParameters, dockerClient, logger) =>
            _parameters = containerParameters;

        public FtpContainer(FtpContainerParameters containerParameters, IContainerApi containerApi, ImageApi imageApi, ILogger? logger)
            : base(containerParameters, containerApi, imageApi, logger) =>
            _parameters = containerParameters;
    }
}
