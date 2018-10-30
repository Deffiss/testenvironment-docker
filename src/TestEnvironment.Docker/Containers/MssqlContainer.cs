using Docker.DotNet;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace TestEnvironment.Docker.Containers
{
    public sealed class MssqlContainer : Container
    {
        private readonly string _saPassword;

        public MssqlContainer(DockerClient dockerClient, string name, string saPassword, string imageName = "microsoft/mssql-server-linux", string tag = "latest", IDictionary<string, string> environmentVariables = null, bool isDockerInDocker = false, bool reuseContainer = false, ILogger logger = null)
            : base(dockerClient, name, imageName, tag,
                new Dictionary<string, string> { ["ACCEPT_EULA"] = "Y", ["SA_PASSWORD"] = saPassword, ["MSSQL_PID"] = "Express" }.MergeDictionaries(environmentVariables),
                isDockerInDocker, reuseContainer, new MssqlContainerWaiter(logger), new MssqlContainerCleaner(logger), logger)
        {
            _saPassword = saPassword;
        }

        public string GetConnectionString() =>
            $"Data Source={(IsDockerInDocker ? IPAddress : "localhost")}, {(IsDockerInDocker ? 1433 : Ports[1433])}; UID=sa; pwd={_saPassword};";
    }
}
