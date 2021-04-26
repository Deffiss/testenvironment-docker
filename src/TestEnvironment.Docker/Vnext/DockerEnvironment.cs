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
#pragma warning disable SA1201 // Elements should appear in the correct order
        public DockerEnvironment()
#pragma warning restore SA1201 // Elements should appear in the correct order
            : this(CreateDefaultDockerClient())
        {
        }

        public DockerEnvironment(ILogger logger)
            : this(new ImageApi(logger), new ContainerApi(logger), logger)
        {
        }

        public DockerEnvironment(IDockerClient dockerClient)
            : this(new ImageApi(dockerClient), new ContainerApi(dockerClient), null)
        {
        }

        public DockerEnvironment(IImageApi imageApi, IContainerApi containerApi, ILogger? logger)
        {
        }

        public ValueTask DisposeAsync()
        {
            throw new NotImplementedException();
        }

        public Task Down(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task Up(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
