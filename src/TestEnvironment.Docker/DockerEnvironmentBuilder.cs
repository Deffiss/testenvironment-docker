using Docker.DotNet;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace TestEnvironment.Docker
{
    public class DockerEnvironmentBuilder : IDockerEnvironmentBuilder
    {
        private readonly List<IDependency> _dependencies = new List<IDependency>();
        private IDictionary<string, string> _variables = new Dictionary<string, string>();

        public DockerClient DockerClient { get; }

        public ILogger Logger { get; private set; } = new LoggerFactory().AddConsole().AddDebug().CreateLogger<DockerEnvironment>();

        public bool IsDockerInDocker { get; private set; } = false;

        public bool DefaultNetwork { get; private set; } = false;

        public string EnvitronmentName { get; private set; } = Guid.NewGuid().ToString().Substring(0, 10);

        public DockerEnvironmentBuilder()
            : this(CreateDefaultDockerClient())
        {
        }

        public DockerEnvironmentBuilder(DockerClient dockerClient)
        {
            DockerClient = dockerClient;
        }

        public IDockerEnvironmentBuilder AddDependency(IDependency dependency)
        {
            if (dependency == null) throw new ArgumentNullException(nameof(dependency));

            _dependencies.Add(dependency);

            return this;
        }

        public IDockerEnvironmentBuilder SetName(string environmentName)
        {
            if (string.IsNullOrEmpty(environmentName)) throw new ArgumentNullException(nameof(environmentName));

            EnvitronmentName = environmentName;

            return this;
        }

        public IDockerEnvironmentBuilder SetVariable(IDictionary<string, string> variables)
        {
            _variables = variables ?? throw new ArgumentNullException(nameof(variables));

            return this;
        }

        public IDockerEnvironmentBuilder AddContainer(string name, string imageName, string tag = "latest", IDictionary<string, string> environmentVariables = null, bool reuseContainer = false, IContainerWaiter containerWaiter = null, IContainerCleaner containerCleaner = null)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            if (string.IsNullOrEmpty(imageName)) throw new ArgumentNullException(nameof(imageName));

            var container = new Container(DockerClient, name.GetContainerName(EnvitronmentName), imageName, tag, environmentVariables, IsDockerInDocker, reuseContainer, containerWaiter, containerCleaner, Logger);
            AddDependency(container);

            return this;
        }

        public IDockerEnvironmentBuilder UseDefaultNetwork()
        {
            DefaultNetwork = true;
            return this;
        }


        public IDockerEnvironmentBuilder DockerInDocker(bool dockerInDocker = true)
        {
            IsDockerInDocker = dockerInDocker;
            return this;
        }

        public IDockerEnvironmentBuilder WithLogger(ILogger logger)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            return this;
        }

        public IDockerEnvironmentBuilder AddFromCompose(Stream composeFileStream) => throw new NotImplementedException();

        public IDockerEnvironmentBuilder AddFromDockerfile(Stream dockerfileStream) => throw new NotImplementedException();

        public DockerEnvironment Build() => new DockerEnvironment(EnvitronmentName, _variables, _dependencies.ToArray(), DockerClient, Logger);

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
