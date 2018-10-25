using Docker.DotNet;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

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
                isDockerInDocker: isDockerInDocker, logger: logger)
        {
            _saPassword = saPassword;
        }

        protected override async Task WaitForReadiness(CancellationToken token = default)
        {
            var attempts = AttemptsCount;
            var isAlive = false;
            do
            {
                try
                {
                    using (var connection = new SqlConnection(GetConnectionString()))
                    using (var command = new SqlCommand("SELECT @@VERSION", connection))
                    {
                        command.Connection.Open();
                        command.ExecuteNonQuery();
                    }

                    isAlive = true;
                }
                catch (Exception ex) when (ex is InvalidOperationException || ex is NotSupportedException || ex is SqlException)
                {
                    Logger.LogDebug(ex.Message);
                }

                if (!isAlive)
                {
                    attempts--;
                    await Task.Delay(DelayTime);
                }
            }
            while (!isAlive && attempts != 0);

            if (attempts == 0)
            {
                Logger.LogError("MSSQL didn't start.");
                throw new TimeoutException("MSSQL didn't start.");
            }
        }

        public string GetConnectionString() =>
            $"Data Source={(IsDockerInDocker ? IPAddress : "localhost")}, {(IsDockerInDocker ? 1433 : Ports[1433])}; UID=SA; pwd={_saPassword};";
    }
}
