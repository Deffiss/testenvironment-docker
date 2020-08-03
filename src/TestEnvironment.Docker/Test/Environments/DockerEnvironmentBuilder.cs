using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace TestEnvironment.Docker.Test.Environments
{
    public class DockerEnvironmentBuilder
    {
        private readonly DockerEnvironmentConfiguration _configuration;

        public DockerEnvironmentBuilder()
        {
            _configuration = new DockerEnvironmentConfiguration();
        }

        public DockerEnvironmentBuilder SetEnvironmentName(string environmentName)
        {
            _configuration.EnvironmentName = environmentName;

            return this;
        }

        public DockerEnvironmentBuilder SetVariables(IDictionary<string, string> environmentVariables)
        {
            _configuration.EnvironmentVariables = environmentVariables;

            return this;
        }

        public DockerEnvironmentBuilder SetIsDockerInDocker(bool isDockerInDocker = true)
        {
            _configuration.IsDockerInDocker = isDockerInDocker;

            return this;
        }

        public DockerEnvironmentBuilder SetLogger(ILogger logger)
        {
            _configuration.Logger = logger;

            return this;
        }

        public DockerEnvironmentBuilder SetIgnoredFolders(params string[] ignoredFolders)
        {
            _configuration.IgnoredFolders = ignoredFolders;

            return this;
        }

        public DockerEnvironmentBuilder AddContainer(Containers.Container container)
        {
            _configuration.Containers.Add(container);

            return this;
        }

        public DockerEnvironment Build()
        {
            return new DockerEnvironment(_configuration);
        }
    }
}
