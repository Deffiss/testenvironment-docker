using System;
using System.Collections.Generic;
using System.Data.OracleClient;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Confluent.Kafka;
using FirebirdSql.Data.FirebirdClient;
using FluentFTP;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MySql.Data.MySqlClient;
using Nest;
using Npgsql;
using TestEnvironment.Docker.Containers.Elasticsearch;
using TestEnvironment.Docker.Containers.Firebird;
using TestEnvironment.Docker.Containers.Ftp;
using TestEnvironment.Docker.Containers.Kafka;
using TestEnvironment.Docker.Containers.Mail;
using TestEnvironment.Docker.Containers.MariaDB;
using TestEnvironment.Docker.Containers.Mongo;
using TestEnvironment.Docker.Containers.Mssql;
using TestEnvironment.Docker.Containers.Oracle;
using TestEnvironment.Docker.Containers.Postgres;
using Xunit;
using Xunit.Abstractions;

namespace TestEnvironment.Docker.Tests
{
    public class DockerEnvironmentTests
    {
        private readonly ITestOutputHelper _testOutput;
        private readonly ILogger _logger;

        public DockerEnvironmentTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
            _logger = new XUnitLogger(testOutput);
        }

        /*

        [Fact]
        public async Task AddKafkaContainer_WhenContainerIsUp_ShouldPrintKafkaVersion()
        {
            // Arrange
            var environment = new DockerEnvironmentBuilder()
                .SetName("test-env")
#if DEBUG
                .AddKafkaContainer("my-kafka", reuseContainer: true)
#else
                .AddKafkaContainer("my-kafka")
#endif
                .Build();

            // Act
            await environment.Up();

            // Assert
            var kafka = environment.GetContainer<KafkaContainer>("my-kafka");
            PrintKafkaVersion(kafka);

            await DisposeEnvironment(environment);
        }

        [Fact]
        public async Task AddElasticSearchContainer_WhenContainerIsUp_ShouldPrintElasticSearchVersion()
        {
            // Arrange
            var environment = new DockerEnvironmentBuilder()
                .UseDefaultNetwork()
                .SetName("test-env")
#if DEBUG
                .AddElasticsearchContainer("my-elastic", ports: new Dictionary<ushort, ushort> { [9200] = 9200 }, reuseContainer: true)
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
                .AddMssqlContainer("my-mssql", "HelloK11tt_0", environmentVariables: new Dictionary<string, string> { ["MSSQL_COLLATION"] = "SQL_Latin1_General_CP1_CS_AS" }, reuseContainer: true)
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
        */

        // Uploaded new oracle-test image 03
        [Fact]
        public async Task AddOracleContainer_WhenContainerIsUp_ShouldPrintOracleVersion()
        {
            Environment.SetEnvironmentVariable("TZ", "UTC");

            // Arrange
            var environment = new DockerEnvironmentBuilder()
                .UseDefaultNetwork()
                .SetName("test-env")
                .AddOracleContainer("my-oracle", userName: "system", password: "oracle", imageName: "oracleinanutshell/oracle-xe-11g", tag: "latest", reuseContainer: false, ports: new Dictionary<ushort, ushort> { [1521] = 1521, [8080] = 8080 }, environmentVariables: new Dictionary<string, string> { { "ORACLE_ALLOW_REMOTE", "true" } })

                // .AddOracleContainer("my-oracle", reuseContainer: false, ports: new Dictionary<ushort, ushort> { [1521] = 1521 }, environmentVariables: new Dictionary<string, string> { { "JAVA_OPTS", "-Doracle.jdbc.timezoneAsRegion=false -Duser.timezone=UTC" } })

                // #if DEBUG
                //                .AddOracleContainer("my-oracle", reuseContainer: false, ports: new Dictionary<ushort, ushort> { [1521] = 1521 },)
                // #else
                //                .AddOracleContainer("my-oracle", ports: new Dictionary<ushort, ushort> { [1521] = 1521 })
                // #endif
                .Build();

            // Act
            await environment.Up();

            // Assert
            var oracle = environment.GetContainer<OracleContainer>("my-oracle");
            await PrintOracleVersion(oracle);

            await DisposeEnvironment(environment);
        }

        /*
        [Fact]
        public async Task AddFirebirdContainer_WhenContainerIsUp_ShouldPrintFirebirdVersion()
        {
            // Arrange
            var environment = new DockerEnvironmentBuilder()
                .UseDefaultNetwork()
                .SetName("test-env")
#if DEBUG
                .AddFirebirdContainer("my-firebird", reuseContainer: false, ports: new Dictionary<ushort, ushort> { [3050] = 3050 })
#else
                .AddFirebirdContainer("my-firebird")
#endif
                .Build();

            // Act
            await environment.Up();

            // Assert
            var firebird = environment.GetContainer<FirebirdContainer>("my-firebird");
            await PrintFirebirdVersion(firebird);

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
                .WithLogger(_logger)
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
        public async Task AddMongoSingleReplicaSetContainer_WhenContainerIsUp_ShouldPrintMongoReplicaSetConfiguration()
        {
            // Arrange
            var environment = new DockerEnvironmentBuilder()
                .UseDefaultNetwork()
                .SetName("test-env")
                .WithLogger(_logger)

                // 27017 port is busy in AppVeyor
#if DEBUG
                .AddMongoSingleReplicaSetContainer("my-mongo-replicaSet", reuseContainer: true, port: 37017)
#else
                .AddMongoSingleReplicaSetContainer("my-mongo-replicaSet", port: 37017)
#endif
                .Build();

            // Act
            await environment.Up();

            // Assert
            var mongo = environment.GetContainer<MongoSingleReplicaSetContainer>("my-mongo-replicaSet");
            await PrintMongoReplicaSetConfiguration(mongo);

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
                .AddFtpContainer(
                    "my-ftp",
                    "superuser",
                    "test",
                    ports: Enumerable.Range(30000, 10).ToDictionary(p => (ushort)p, p => (ushort)p).MergeDictionaries(new Dictionary<ushort, ushort> { [21] = 21 }),
                    reuseContainer: true)
#else
                .AddFtpContainer(
                    "my-ftp",
                    "superuser",
                    "test",
                    ports: Enumerable.Range(30000, 10)
                        .ToDictionary(p => (ushort)p, p => (ushort)p).MergeDictionaries(new Dictionary<ushort, ushort>
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
                .AddFromDockerfile("from-file", "Dockerfile", containerWaiter: new HttpContainerWaiter("/", httpPort: 8080), reuseContainer: true)
#else
                .AddFromDockerfile("from-file", "Dockerfile", containerWaiter: new HttpContainerWaiter("/", httpPort: 8080))
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

        [Fact]
        public async Task TwoContainersWithSimilarNames_ShouldStartCorrectly()
        {
            // Arrange
            var environment = new DockerEnvironmentBuilder()
                .UseDefaultNetwork()
                .SetName("test-env-similar-names")
#if DEBUG
                .AddMongoContainer("my-mongo-2", reuseContainer: true)
                .AddMongoContainer("my-mongo", tag: "4.0", reuseContainer: true)
#else
                .AddMongoContainer("my-mongo-2")
                .AddMongoContainer("my-mongo", tag: "4.0")
#endif
                .Build();

            // Act
            await environment.Up();

            // Assert
            var mongo = environment.GetContainer<MongoContainer>("my-mongo");
            var mongo2 = environment.GetContainer<MongoContainer>("my-mongo-2");
            PrintMongoVersion(mongo);
            PrintMongoVersion(mongo2);

            await DisposeEnvironment(environment);
        }
        */

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

        private void PrintKafkaVersion(KafkaContainer kafka)
        {
            using var adminClient = new AdminClientBuilder(new AdminClientConfig
            {
                BootstrapServers = kafka.GetUrl(),
                ApiVersionRequestTimeoutMs = 50000
            })
                .Build();

            var metaData = adminClient.GetMetadata(TimeSpan.FromSeconds(1));
            _testOutput.WriteLine($"Kafka version: {metaData.Brokers.FirstOrDefault().ToString()}");
        }

        private async Task PrintOracleVersion(OracleContainer oracle)
        {
            using var connection = new OracleConnection(oracle.GetConnectionString());
            using var command = new OracleCommand("SELECT * FROM V$VERSION", connection);
            await connection.OpenAsync();

            var reader = await command.ExecuteReaderAsync();
            await reader.ReadAsync();

            _testOutput.WriteLine($"Oracle Version: {reader.GetString(0)}");
        }

        private async Task PrintFirebirdVersion(FirebirdContainer firebird)
        {
            using (var connection = new FbConnection(firebird.GetConnectionString()))
            using (var command = new FbCommand("SELECT rdb$get_context('SYSTEM', 'ENGINE_VERSION') from rdb$database;", connection))
            {
                await connection.OpenAsync();

                var reader = await command.ExecuteReaderAsync();
                await reader.ReadAsync();

                _testOutput.WriteLine($"Firebird Version: {reader.GetString(0)}");
            }
        }

        private async Task PrintElasticsearchVersion(ElasticsearchContainer elastic)
        {
            var elasticClient = new ElasticClient(new Uri(elastic.GetUrl()));
            var clusterInfo = await elasticClient.Nodes.InfoAsync();
            _testOutput.WriteLine($"Elasticsearch version: {clusterInfo.Nodes.Values.First().Version}");
        }

        private void PrintMongoVersion(MongoContainer mongo)
        {
            var mongoClient = new MongoClient(mongo.GetConnectionString());
            var clusterDescription = mongoClient.Cluster.Description;
            _testOutput.WriteLine($"Mongo version: {clusterDescription.Servers.First().Version}");
        }

        private async Task PrintMongoReplicaSetConfiguration(MongoSingleReplicaSetContainer mongo)
        {
            var mongoClient = new MongoClient(mongo.GetConnectionString());

            var configuration = await mongoClient.GetDatabase("admin")
                .RunCommandAsync(new BsonDocumentCommand<BsonDocument>(new BsonDocument { { "replSetGetConfig", 1 } }));

            _testOutput.WriteLine($"Mongo replica set configuration: {configuration}");
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

            using (var ftpClient = new FtpClient(ftpContainer.FtpHost, port, ftpContainer.FtpUserName, ftpContainer.FtpPassword))
            {
                await ftpClient.ConnectAsync();

                _testOutput.WriteLine($"FTP type: {ftpClient.ServerType}");
            }
        }

        private async Task PrintSmtpCapabilities(MailContainer mailContainer)
        {
            var host = mailContainer.IsDockerInDocker ? mailContainer.IPAddress : IPAddress.Loopback.ToString();
            var port = mailContainer.IsDockerInDocker ? 1025 : mailContainer.Ports[1025];

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(host, port);

                _testOutput.WriteLine($"Smtp capabilites: {client.Capabilities}");
            }
        }

        private async Task PrintReturnedHtml(Container staticFilesContainer)
        {
            var host = staticFilesContainer.IsDockerInDocker ? staticFilesContainer.IPAddress : IPAddress.Loopback.ToString();
            var port = staticFilesContainer.IsDockerInDocker ? 8080 : staticFilesContainer.Ports[8080];

            using (var client = new HttpClient { BaseAddress = new Uri($"http://{host}:{port}") })
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
            await environment.Down();

            await environment.DisposeAsync();
        }
    }
}
