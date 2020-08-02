using System;
using System.Runtime.InteropServices;
using Docker.DotNet;

namespace TestEnvironment.Docker.Test.Helpers
{
    public static class DockerClientStorage
    {
        private static readonly Lazy<DockerClient> _dockerClient = new Lazy<DockerClient>(CreateDefaultDockerClient, true);

        public static DockerClient DockerClient => _dockerClient.Value;

        private static DockerClient CreateDefaultDockerClient()
        {
            var dockerHostVar = Environment.GetEnvironmentVariable("DOCKER_HOST");
            var defaultDockerUrl = !string.IsNullOrEmpty(dockerHostVar)
                ? dockerHostVar
                : !RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                    ? "unix:///var/run/docker.sock"
                    : "npipe://./pipe/docker_engine";

            return new DockerClientConfiguration(new Uri(defaultDockerUrl)).CreateClient();
        }
    }
}
