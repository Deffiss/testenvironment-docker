using Docker.DotNet;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace TestEnvironment.Docker.Containers
{
    public sealed class MssqlContainer : Container
    {
        private const int AttemptsCount = 60;
        private const int DelayTime = 1000;

        private readonly string _saPassword;

        public MssqlContainer(DockerClient dockerClient, string name, string saPassword, string imageName = "microsoft/mssql-server-linux", string tag = "latest", ILogger logger = null, bool isDockerInDocker = false)
            : base(dockerClient, name, imageName, tag,
                environmentVariables: new Dictionary<string, string> { ["ACCEPT_EULA"] = "Y", ["SA_PASSWORD"] = saPassword, ["MSSQL_PID"] = "Express" },
                isDockerInDocker: isDockerInDocker, new MssqlContainerWaiter(), logger: logger)
        {
            _saPassword = saPassword;
        }

        public string GetConnectionString() =>
            $"Data Source={(IsDockerInDocker ? IPAddress : "localhost")}, {(IsDockerInDocker ? 1433 : Ports[1433])}; UID=SA; pwd={_saPassword};";
    }
}
