using Docker.DotNet;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace TestEnvironment.Docker.Containers
{
    public sealed class MssqlContainer : Container, ICleanable
    {
        private const string CleanupCommand = "EXEC sp_MSforeachdb " +
            @"'IF DB_ID(''?'') > 4 BEGIN
                ALTER DATABASE [?] SET SINGLE_USER WITH ROLLBACK IMMEDIATE
                DROP DATABASE [?]
            END'";

        private readonly string _saPassword;

        public MssqlContainer(DockerClient dockerClient, string name, string saPassword, string imageName = "microsoft/mssql-server-linux", string tag = "latest", bool isDockerInDocker = false, bool reuseContainer = false, ILogger logger = null)
            : base(dockerClient, name, imageName, tag,
                new Dictionary<string, string> { ["ACCEPT_EULA"] = "Y", ["SA_PASSWORD"] = saPassword, ["MSSQL_PID"] = "Express" },
                isDockerInDocker, new MssqlContainerWaiter(), reuseContainer, logger)
        {
            _saPassword = saPassword;
        }

        public async Task Cleanup(CancellationToken token = default)
        {
            using (var connection = new SqlConnection(GetConnectionString()))
            using (var command = new SqlCommand(CleanupCommand, connection))
            {
                await command.Connection.OpenAsync();

                try
                {
                    await command.ExecuteNonQueryAsync();
                }
                catch (SqlException e)
                {
                    Logger.LogInformation($"Cleanup issue: {e.Message}");
                }
            }
        }

        public string GetConnectionString() =>
            $"Data Source={(IsDockerInDocker ? IPAddress : "localhost")}, {(IsDockerInDocker ? 1433 : Ports[1433])}; UID=SA; pwd={_saPassword};";
    }
}
