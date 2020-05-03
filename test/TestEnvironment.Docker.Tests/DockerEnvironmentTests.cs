using MongoDB.Driver;
using Nest;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TestEnvironment.Docker.Containers.Elasticsearch;
using TestEnvironment.Docker.Containers.Ftp;
using TestEnvironment.Docker.Containers.MariaDB;
using TestEnvironment.Docker.Containers.Mail;
using TestEnvironment.Docker.Containers.Mongo;
using TestEnvironment.Docker.Containers.Mssql;
using TestEnvironment.Docker.Containers.Postgres;
using Xunit;
using MySql.Data.MySqlClient;
using FluentFTP;
using MailKit.Net.Smtp;
using Npgsql;
using Xunit.Abstractions;

namespace TestEnvironment.Docker.Tests
{
    public class DockerEnvironmentTests
    {
        private readonly ITestOutputHelper _testOutput;

        public DockerEnvironmentTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        [Fact]
        public async Task AddElasticSearchContainer_WhenContainerIsUp_ShouldPrintElasticSearchVersion()
        {
            // Arrange
            var environment = new DockerEnvironmentBuilder()
                .UseDefaultNetwork()
                .SetName("test-env")
#if DEBUG
                .AddElasticsearchContainer("my-elastic", ports: new Dictionary<ushort, ushort> { [9200] = 9200 },
                    reuseContainer: true)
#else
                .AddElasticsearchContainer("my-elastic")
#endif
                .Build();

            // Act
            await environment.Up();

            // Assert
            var elastic = environment.GetContainer<ElasticsearchContainer>("my-elastic");
            await PrintElasticsearchVersion(elastic);

            await DisposeEnvironment(environment);
        }

        [Fact]
        public async Task AddMsSqlContainer_WhenContainerIsUp_ShouldPrintMsSqlVersion()
        {
            // Arrange
            var environment = new DockerEnvironmentBuilder()
                .UseDefaultNetwork()
                .SetName("test-env")
#if DEBUG
                .AddMssqlContainer("my-mssql", "HelloK11tt_0",
                    environmentVariables: new Dictionary<string, string>
                    { ["MSSQL_COLLATION"] = "SQL_Latin1_General_CP1_CS_AS" }, reuseContainer: true)
#else
                .AddMssqlContainer("my-mssql", "HelloK11tt_0")
#endif
                .Build();

            // Act
            await environment.Up();

            // Assert
            var mssql = environment.GetContainer<MssqlContainer>("my-mssql");
            await PrintMssqlVersion(mssql);

            await DisposeEnvironment(environment);
        }

        [Fact]
        public async Task AddMariaDbContainer_WhenContainerIsUp_ShouldPrintMariaDbVersion()
        {
            // Arrange
            var environment = new DockerEnvironmentBuilder()
                .UseDefaultNetwork()
                .SetName("test-env")
#if DEBUG
                .AddMariaDBContainer("my-maria", "my-secret-pw", reuseContainer: true)
#else
                .AddMariaDBContainer("my-maria", "my-secret-pw")
#endif
                .Build();

            // Act
            await environment.Up();

            // Assert
            var maria = environment.GetContainer<MariaDBContainer>("my-maria");
            await PrintMariaDBVersion(maria);

            await DisposeEnvironment(environment);
        }

        [Fact]
        public async Task AddMongoContainer_WhenContainerIsUp_ShouldPrintMongoVersion()
        {
            // Arrange
            var environment = new DockerEnvironmentBuilder()
                .UseDefaultNetwork()
                .SetName("test-env")
#if DEBUG
                .AddMongoContainer("my-mongo", reuseContainer: true)
#else
                .AddMongoContainer("my-mongo")
#endif
                .Build();

            // Act
            await environment.Up();

            // Assert
            var mongo = environment.GetContainer<MongoContainer>("my-mongo");
            PrintMongoVersion(mongo);

            await DisposeEnvironment(environment);
        }

        [Fact]
        public async Task AddMailContainer_WhenContainerIsUp_ShouldPrintSmtpCapabilities()
        {
            // Arrange
            var environment = new DockerEnvironmentBuilder()
                .UseDefaultNetwork()
                .SetName("test-env")
#if DEBUG
                .AddMailContainer("my-mail", reuseContainer: true)
#else
                .AddMailContainer("my-mail")
#endif
                .Build();

            // Act
            await environment.Up();

            // Assert
            var mail = environment.GetContainer<MailContainer>("my-mail");
            await PrintSmtpCapabilities(mail);

            await DisposeEnvironment(environment);
        }

        [Fact]
        public async Task AddFtpContainer_WhenContainerIsUp_ShouldPrintFtpServerType()
        {
            // Arrange
            var environment = new DockerEnvironmentBuilder()
                .UseDefaultNetwork()
                .SetName("test-env")
#if DEBUG
                .AddFtpContainer("my-ftp", "superuser", "test",
                    ports: Enumerable.Range(30000, 10).ToDictionary(p => (ushort)p, p => (ushort)p)
                        .MergeDictionaries(new Dictionary<ushort, ushort> { [21] = 21 }), reuseContainer: true)
#else
                .AddFtpContainer("my-ftp", "superuser", "test", ports: Enumerable.Range(30000, 10)
                    .ToDictionary(p => (ushort) p, p => (ushort) p).MergeDictionaries(new Dictionary<ushort, ushort>
                    {
                        [21] = 21
                    }))
#endif
                .Build();

            // Act
            await environment.Up();

            // Assert
            var ftp = environment.GetContainer<FtpContainer>("my-ftp");
            await PrintFtpServerType(ftp);

            await DisposeEnvironment(environment);
        }

        [Fact]
        public async Task AddFromDockerFileContainer_WhenContainerIsUp_ShouldPrintReturnedHtml()
        {
            // Arrange
            var environment = new DockerEnvironmentBuilder()
                .UseDefaultNetwork()
                .SetName("test-env")
#if DEBUG
                .AddFromDockerfile("from-file", "Dockerfile",
                    containerWaiter: new HttpContainerWaiter("/", httpPort: 8080), reuseContainer: true)
#else
                .AddFromDockerfile("from-file", "Dockerfile",
                    containerWaiter: new HttpContainerWaiter("/", httpPort: 8080))
#endif
                .Build();

            // Act
            await environment.Up();

            // Assert
            var staticFilesContainer = environment.GetContainer("from-file");
            await PrintReturnedHtml(staticFilesContainer);

            await DisposeEnvironment(environment);
        }

        [Fact]
        public async Task AddPostgresContainer_WhenContainerIsUp_ShouldPrintPostgresDbVersion()
        {
            // Arrange
            var environment = new DockerEnvironmentBuilder()
                .UseDefaultNetwork()
                .SetName("test-env")
#if DEBUG
                .AddPostgresContainer("my-postgres", reuseContainer: true)
#else
                .AddPostgresContainer("my-postgres")
#endif
                .Build();

            // Act
            await environment.Up();

            // Assert
            var postgres = environment.GetContainer<PostgresContainer>("my-postgres");
            await PrintPostgresDbVersion(postgres);

            await DisposeEnvironment(environment);
        }

        private async Task PrintMssqlVersion(MssqlContainer mssql)
        {
            using (var connection = new SqlConnection(mssql.GetConnectionString()))
            using (var command = new SqlCommand("SELECT @@VERSION", connection))
            {
                await connection.OpenAsync();

                var reader = await command.ExecuteReaderAsync();
                await reader.ReadAsync();

                _testOutput.WriteLine($"MSSQL Version: {reader.GetString(0)}");
            }
        }

        private async Task PrintElasticsearchVersion(ElasticsearchContainer elastic)
        {
            var elasticClient = new ElasticClient(new Uri(elastic.GetUrl()));
            var clusterInfo = await elasticClient.NodesInfoAsync();
            _testOutput.WriteLine($"Elasticsearch version: {clusterInfo.Nodes.Values.First().Version}");
        }

        private void PrintMongoVersion(MongoContainer mongo)
        {
            var mongoClient = new MongoClient(mongo.GetConnectionString());
            var clusterDescription = mongoClient.Cluster.Description;
            _testOutput.WriteLine($"Mongo version: {clusterDescription.Servers.First().Version}");
        }

        private async Task PrintMariaDBVersion(MariaDBContainer maria)
        {
            using (var connection = new MySqlConnection(maria.GetConnectionString()))
            using (var command = new MySqlCommand("select @@version", connection))
            {
                await command.Connection.OpenAsync();

                var reader = await command.ExecuteReaderAsync();
                await reader.ReadAsync();

                _testOutput.WriteLine($"MariaDB Version: {reader.GetString(0)}");
            }
        }

        private async Task PrintFtpServerType(FtpContainer ftpContainer)
        {
            var port = ftpContainer.IsDockerInDocker ? 21 : ftpContainer.Ports[21];

            using (var ftpClient = new FtpClient(ftpContainer.FtpHost, port, ftpContainer.FtpUserName,
                ftpContainer.FtpPassword))
            {
                await ftpClient.ConnectAsync();

                _testOutput.WriteLine($"FTP type: {ftpClient.ServerType}");
            }
        }

        private async Task PrintSmtpCapabilities(MailContainer mailContainer)
        {
            var host = mailContainer.IsDockerInDocker ? mailContainer.IPAddress : "localhost";
            var port = mailContainer.IsDockerInDocker ? 1025 : mailContainer.Ports[1025];

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(host, port);

                _testOutput.WriteLine($"Smtp capabilites: {client.Capabilities}");
            }
        }

        private async Task PrintReturnedHtml(Container staticFilesContainer)
        {
            var host = staticFilesContainer.IsDockerInDocker ? staticFilesContainer.IPAddress : "localhost";
            var port = staticFilesContainer.IsDockerInDocker ? 8080 : staticFilesContainer.Ports[8080];

            using (var client = new HttpClient {BaseAddress = new Uri($"http://{host}:{port}")})
            {
                var response = await client.GetStringAsync("/");
                _testOutput.WriteLine($"Response from static server: {response}");
            }
        }

        private async Task PrintPostgresDbVersion(PostgresContainer postgres)
        {
            using (var connection = new NpgsqlConnection(postgres.GetConnectionString()))
            using (var command = new NpgsqlCommand("select version();", connection))
            {
                await connection.OpenAsync();

                var reader = await command.ExecuteReaderAsync();
                await reader.ReadAsync();

                _testOutput.WriteLine($"Postgres Version: {reader.GetString(0)}");
            }
        }

        private async Task DisposeEnvironment(DockerEnvironment environment)
        {
#if !DEBUG
            await environment.Down();
            await environment.DisposeAsync();
#endif
        }
    }
}
