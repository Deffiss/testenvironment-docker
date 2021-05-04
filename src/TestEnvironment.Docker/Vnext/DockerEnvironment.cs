using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet;
using Microsoft.Extensions.Logging;
using TestEnvironment.Docker.Vnext.ContainerOperations;
using TestEnvironment.Docker.Vnext.ImageOperations;
using static TestEnvironment.Docker.Vnext.DockerClientExtentions;

namespace TestEnvironment.Docker.Vnext
{
    public class DockerEnvironment : IDockerEnvironment
    {
        private readonly IImageApi _imageApi;
        private readonly IContainerApi _containerApi;
        private readonly ILogger? _logger;

        public Container[] Containers { get; init; }

#pragma warning disable SA1201 // Elements should appear in the correct order
        public DockerEnvironment(Container[] containers)
#pragma warning restore SA1201 // Elements should appear in the correct order
            : this(containers, CreateDefaultDockerClient())
        {
        }

        public DockerEnvironment(Container[] containers, ILogger logger)
            : this(containers, new ImageApi(logger), new ContainerApi(logger), logger)
        {
        }

        public DockerEnvironment(Container[] containers, IDockerClient dockerClient)
            : this(containers, new ImageApi(dockerClient), new ContainerApi(dockerClient), null)
        {
        }

        public DockerEnvironment(Container[] containers, IImageApi imageApi, IContainerApi containerApi, ILogger? logger) =>
            (Containers, _imageApi, _containerApi, _logger) = (containers, imageApi, containerApi, logger);

        public Task Up(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task Down(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public ValueTask DisposeAsync()
        {
            throw new NotImplementedException();
        }
    }
}
