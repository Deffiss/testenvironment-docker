using Docker.DotNet;
using System;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace TestEnvironment.Docker.Containers
{
    public sealed class MssqlContainer : Container
    {
        private const int AttemptsCount = 60;
        private const int DelayTime = 500;

        private readonly string _saPassword;

        public MssqlContainer(DockerClient dockerClient, string name, string saPassword, Action<string> logger = null)
            : base(dockerClient, name, "microsoft/mssql-server-linux",
                environmentVariables: new[] { ("ACCEPT_EULA", "Y"), ("SA_PASSWORD", saPassword), ("MSSQL_PID", "Express") },
                logger: logger)
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
                    Logger?.Invoke(ex.Message);
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
                throw new TimeoutException("MSSQL didn't start");
            }
        }

        public string GetConnectionString(bool isInternal = false) =>
            $"Data Source={(isInternal ? IpAddress : "localhost")}, {(isInternal ? 1433 : Ports[1433])}; UID=SA; pwd={_saPassword};";
    }
}
