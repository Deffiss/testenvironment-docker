using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet;
using Microsoft.Extensions.Logging;
using TestEnvironment.Docker.Vnext.ContainerOperations;
using TestEnvironment.Docker.Vnext.ImageOperations;

namespace TestEnvironment.Docker.Vnext
{
    public class ContainerFromDockerfile : Container
    {
        private readonly ContainerFromDockerfileParameters _parameters;

        public ContainerFromDockerfile(ContainerFromDockerfileParameters containerParameters)
            : base(containerParameters) =>
            _parameters = containerParameters;

        public ContainerFromDockerfile(ContainerFromDockerfileParameters containerParameters, IDockerClient dockerClient)
            : base(containerParameters, dockerClient) =>
            _parameters = containerParameters;

        public ContainerFromDockerfile(ContainerFromDockerfileParameters containerParameters, IDockerClient dockerClient, ILogger? logger)
            : base(containerParameters, dockerClient, logger) =>
            _parameters = containerParameters;

        public ContainerFromDockerfile(ContainerFromDockerfileParameters containerParameters, IContainerApi containerApi, ImageApi imageApi, ILogger? logger)
            : base(containerParameters, containerApi, imageApi, logger) =>
            _parameters = containerParameters;

        public override async Task EnsureImageAvailableAsync(CancellationToken cancellationToken = default) =>
            await ImageApi.BuildImageAsync(_parameters.Dockerfile, ImageName, Tag, _parameters.Context, _parameters.BuildArgs, _parameters.IgnoredContextFiles, cancellationToken);
    }
}
