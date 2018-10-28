using Docker.DotNet;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace TestEnvironment.Docker.Containers
{
    public sealed class MssqlContainer : Container
    {
        private readonly string _saPassword;

        public MssqlContainer(DockerClient dockerClient, string name, string saPassword, string imageName = "microsoft/mssql-server-linux", string tag = "latest", bool isDockerInDocker = false, bool reuseContainer = false, ILogger logger = null)
            : base(dockerClient, name, imageName, tag,
                new Dictionary<string, string> { ["ACCEPT_EULA"] = "Y", ["SA_PASSWORD"] = saPassword, ["MSSQL_PID"] = "Express" },
                isDockerInDocker, reuseContainer, new MssqlContainerWaiter(logger), new MssqlContainerCleaner(logger), logger)
        {
            _saPassword = saPassword;
        }

        public string GetConnectionString() =>
            $"Data Source={(IsDockerInDocker ? IPAddress : "localhost")}, {(IsDockerInDocker ? 1433 : Ports[1433])}; UID=SA; pwd={_saPassword};";
    }
}
