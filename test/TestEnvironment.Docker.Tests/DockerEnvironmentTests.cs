using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using FluentFTP;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MySqlConnector;
using Nest;
using Npgsql;
using RabbitMQ.Client;
using StackExchange.Redis;
using TestEnvironment.Docker.ContainerLifecycle;
using TestEnvironment.Docker.Containers.Elasticsearch;
using TestEnvironment.Docker.Containers.Ftp;
using TestEnvironment.Docker.Containers.Kafka;
using TestEnvironment.Docker.Containers.Mail;
using TestEnvironment.Docker.Containers.MariaDB;
using TestEnvironment.Docker.Containers.Mongo;
using TestEnvironment.Docker.Containers.Mssql;
using TestEnvironment.Docker.Containers.Postgres;
using TestEnvironment.Docker.Containers.RabbitMQ;
using TestEnvironment.Docker.Containers.Redis;
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
            _logger = LoggerFactory.Create(lb => lb.AddConsole().AddDebug())
                .CreateLogger<DockerEnvironment>();
        }

        [Fact]
        public async Task AddRedisContainer_WhenContainerIsUp_ShouldPrintStats()
        {
#if DEBUG
            var environment = new DockerEnvironmentBuilder(_logger)
#else
            await using var environment = new DockerEnvironmentBuilder(_logger)
#endif
                .SetName("test-env")
#if WSL2
                .UseWsl2()
#endif
#if DEBUG
                .AddRedisContainer(p => p with { Name = "my-redis", Reusable = false })
#else
                .AddRedisContainer(p => p with { Name = "my-redis" })
#endif
                .Build();

            // Act
            await environment.UpAsync();

            // Assert
            var redis = environment.GetContainer<RedisContainer>("my-redis");
            await PrintRedisStats(redis);

            await environment.DownAsync();
        }

        [Fact]
        public async Task AddKafkaContainer_WhenContainerIsUp_ShouldPrintKafkaVersion()
        {
            // Arrange
#if DEBUG
            var environment = new DockerEnvironmentBuilder(_logger)
#else
            await using var environment = new DockerEnvironmentBuilder(_logger)
#endif
                .SetName("test-env")
#if WSL2
                .UseWsl2()
#endif
#if DEBUG
                .AddKafkaContainer(p => p with { Name = "my-kafka", Reusable = true })
#else
                .AddKafkaContainer(p => p with { Name = "my-kafka" })
#endif
                .Build();

            // Act
            await environment.UpAsync();

            // Assert
            var kafka = environment.GetContainer<KafkaContainer>("my-kafka");
            PrintKafkaVersion(kafka);
        }

        [Fact]
        public async Task AddElasticSearchContainer_WhenContainerIsUp_ShouldPrintElasticSearchVersion()
        {
            // Arrange
#if DEBUG
            var environment = new DockerEnvironmentBuilder(_logger)
#else
            await using var environment = new DockerEnvironmentBuilder(_logger)
#endif
                .SetName("test-env")
#if WSL2
                .UseWsl2()
#endif
#if DEBUG
                .AddElasticsearchContainer(p => p with
                {
                    Name = "my-elastic",
                    Ports = new Dictionary<ushort, ushort> { [9200] = 9200 },
                    Reusable = true
                })
#else
                .AddElasticsearchContainer(p => p with { Name = "my-elastic" })
#endif
                .Build();

            // Act
            await environment.UpAsync();

            // Assert
            var elastic = environment.GetContainer<ElasticsearchContainer>("my-elastic");
            await PrintElasticsearchVersion(elastic);
        }

        [Fact]
        public async Task AddMsSqlContainer_WhenContainerIsUp_ShouldPrintMsSqlVersion()
        {
            // Arrange
#if DEBUG
            var environment = new DockerEnvironmentBuilder(_logger)
#else
            await using var environment = new DockerEnvironmentBuilder(_logger)
#endif
                .SetName("test-env")
#if WSL2
                .UseWsl2()
#endif
#if DEBUG
                .AddMssqlContainer(p => p with
                {
                    Name = "my-mssql",
                    SAPassword = "HelloK11tt_0",
                    EnvironmentVariables = new Dictionary<string, string>
                    {
                        ["MSSQL_COLLATION"] = "SQL_Latin1_General_CP1_CS_AS"
                    },
                    Reusable = true
                })
#else
                .AddMssqlContainer(p => p with
                {
                    Name = "my-mssql",
                    SAPassword = "HelloK11tt_0",
                    EnvironmentVariables = new Dictionary<string, string>
                    {
                        ["MSSQL_COLLATION"] = "SQL_Latin1_General_CP1_CS_AS"
                    }
                })
#endif
                .Build();

            // Act
            await environment.UpAsync();

            // Assert
            var mssql = environment.GetContainer<MssqlContainer>("my-mssql");
            await PrintMssqlVersion(mssql);
        }

        [Fact]
        public async Task AddMsSqlContainerWithCustomApi_WhenContainerIsUp_ShouldHaveCustomName()
        {
            // Arrange
            const string nameSuffix = "custom";
            const string containerName = "my-mssql";

#if DEBUG
            var environment = new DockerEnvironmentBuilder(_logger)
#else
            await using var environment = new DockerEnvironmentBuilder(_logger)
#endif
                .SetName("test-env")
                .WithContainerApi((api, l) => new CustomContainerApi(nameSuffix, api, l))
#if WSL2
                .UseWsl2()
#endif
#if DEBUG
                .AddMssqlContainer(p => p with
                {
                    Name = containerName,
                    SAPassword = "HelloK11tt_0",
                    Reusable = true
                })
#else
                .AddMssqlContainer(p => p with
                {
                    Name = "my-mssql",
                    SAPassword = "HelloK11tt_0"
                })
#endif
                .Build();

            // Act
            await environment.UpAsync();

            // Assert
            var mssql = environment.GetContainer<MssqlContainer>("my-mssql");
            Assert.EndsWith(mssql.Name, nameSuffix);
        }

        [Fact]
        public async Task AddMariaDbContainer_WhenContainerIsUp_ShouldPrintMariaDbVersion()
        {
            // Arrange
#if DEBUG
            var environment = new DockerEnvironmentBuilder(_logger)
#else
            await using var environment = new DockerEnvironmentBuilder(_logger)
#endif
                .SetName("test-env")
#if WSL2
                .UseWsl2()
#endif
#if DEBUG
                .AddMariaDBContainer(p => p with
                {
                    Name = "my-maria",
                    RootPassword = "my-secret-pw",
                    Reusable = true
                })
#else
                .AddMariaDBContainer(p => p with
                {
                    Name = "my-maria",
                    RootPassword = "my-secret-pw"
                })
#endif
                .Build();

            // Act
            await environment.UpAsync();

            // Assert
            var maria = environment.GetContainer<MariaDBContainer>("my-maria");
            await PrintMariaDBVersion(maria);
        }

        [Fact]
        public async Task AddMongoContainer_WhenContainerIsUp_ShouldPrintMongoVersion()
        {
            // Arrange
#if DEBUG
            var environment = new DockerEnvironmentBuilder(_logger)
#else
            await using var environment = new DockerEnvironmentBuilder(_logger)
#endif
                .SetName("test-env")
#if WSL2
                .UseWsl2()
#endif
#if DEBUG
                .AddMongoContainer(p => p with
                {
                    Name = "my-mongo",
                    Reusable = true
                })
#else
                .AddMongoContainer(p => p with
                {
                    Name = "my-mongo"
                })
#endif
                .Build();

            // Act
            await environment.UpAsync();

            // Assert
            var mongo = environment.GetContainer<MongoContainer>("my-mongo");
            PrintMongoVersion(mongo);
        }

        [Fact]
        public async Task AddMongoSingleReplicaSetContainer_WhenContainerIsUp_ShouldPrintMongoReplicaSetConfiguration()
        {
            // Arrange
#if DEBUG
            var environment = new DockerEnvironmentBuilder(_logger)
#else
            await using var environment = new DockerEnvironmentBuilder(_logger)

#endif
                .SetName("test-env")
#if WSL2
                .UseWsl2()
#endif

                // 27017 port is busy in AppVeyor
#if DEBUG
                .AddMongoSingleReplicaSetContainer(p => p with
                {
                    Name = "my-mongo-replicaSet",
                    CustomReplicaSetPort = 37017,
                    Reusable = true
                })
#else
                .AddMongoSingleReplicaSetContainer(p => p with
                {
                    Name = "my-mongo-replicaSet",
                    CustomReplicaSetPort = 37017
                })
#endif
                .Build();

            // Act
            await environment.UpAsync();

            // Assert
            var mongo = environment.GetContainer<MongoSingleReplicaSetContainer>("my-mongo-replicaSet");
            await PrintMongoReplicaSetConfiguration(mongo);
        }

        [Fact]
        public async Task AddMailContainer_WhenContainerIsUp_ShouldPrintSmtpCapabilities()
        {
            // Arrange
#if DEBUG
            var environment = new DockerEnvironmentBuilder(_logger)
#else
            await using var environment = new DockerEnvironmentBuilder(_logger)
#endif
                .SetName("test-env")
#if WSL2
                .UseWsl2()
#endif
#if DEBUG
                .AddMailContainer(p => p with
                {
                    Name = "my-mail",
                    Reusable = true
                })
#else
                .AddMailContainer(p => p with
                {
                    Name = "my-mail"
                })
#endif
                .Build();

            // Act
            await environment.UpAsync();

            // Assert
            var mail = environment.GetContainer<MailContainer>("my-mail");
            await PrintSmtpCapabilities(mail);
        }

        [Fact]
        public async Task AddFtpContainer_WhenContainerIsUp_ShouldPrintFtpServerType()
        {
            // Arrange
#if DEBUG
            var environment = new DockerEnvironmentBuilder(_logger)
#else
            await using var environment = new DockerEnvironmentBuilder(_logger)
#endif
                .SetName("test-env")
#if WSL2
                .UseWsl2()
#endif
#if DEBUG
                .AddFtpContainer(p => p with
                {
                    Name = "my-ftp",
                    FtpUserName = "superuser",
                    FtpPassword = "test",
                    Ports = Enumerable.Range(30000, 10).ToDictionary(p => (ushort)p, p => (ushort)p).MergeDictionaries(new Dictionary<ushort, ushort> { [21] = 21 }),
                    Reusable = true
                })
#else
                .AddFtpContainer(p => p with
                {
                    Name = "my-ftp",
                    FtpUserName = "superuser",
                    FtpPassword = "test",
                    Ports = Enumerable.Range(30000, 10).ToDictionary(p => (ushort)p, p => (ushort)p).MergeDictionaries(new Dictionary<ushort, ushort> { [21] = 21 })
                })
#endif
                .Build();

            // Act
            await environment.UpAsync();

            // Assert
            var ftp = environment.GetContainer<FtpContainer>("my-ftp");
            await PrintFtpServerType(ftp);
        }

        [Fact]
        public async Task AddFromDockerFileContainer_WhenContainerIsUp_ShouldPrintReturnedHtml()
        {
            // Arrange
#if DEBUG
            var environment = new DockerEnvironmentBuilder(_logger)
#else
            await using var environment = new DockerEnvironmentBuilder(_logger)
#endif
                .SetName("test-env")
#if WSL2
                .UseWsl2()
#endif
#if DEBUG
                .AddFromDockerfile(p => p with
                {
                    Name = "from-file",
                    Dockerfile = "Dockerfile",
                    ContainerWaiter = new HttpContainerWaiter("/", port: 8080),
                    Reusable = true
                })
#else
                .AddFromDockerfile(p => p with
                {
                    Name = "from-file",
                    Dockerfile = "Dockerfile",
                    ContainerWaiter = new HttpContainerWaiter("/", port: 8080)
                })
#endif
                .Build();

            // Act
            await environment.UpAsync();

            // Assert
            var staticFilesContainer = environment.GetContainer("from-file");
            await PrintReturnedHtml(staticFilesContainer);
        }

        [Fact]
        public async Task AddPostgresContainer_WhenContainerIsUp_ShouldPrintPostgresDbVersion()
        {
            // Arrange
#if DEBUG
            var environment = new DockerEnvironmentBuilder(_logger)
#else
            await using var environment = new DockerEnvironmentBuilder(_logger)

#endif
                .SetName("test-env")
#if WSL2
                .UseWsl2()
#endif
#if DEBUG
                .AddPostgresContainer(p => p with
                {
                    Name = "my-postgres",
                    Reusable = true
                })
#else
                .AddPostgresContainer(p => p with
                {
                    Name = "my-postgres"
                })
#endif
                .Build();

            // Act
            await environment.UpAsync();

            // Assert
            var postgres = environment.GetContainer<PostgresContainer>("my-postgres");
            await PrintPostgresDbVersion(postgres);
        }

        [Fact]
        public async Task AddRabbitMqContainer_WhenContainerIsUp_ShouldPrintServerInformation()
        {
            // Arrange
#if DEBUG
            var environment = new DockerEnvironmentBuilder(_logger)
#else
            await using var environment = new DockerEnvironmentBuilder(_logger)

#endif
                .SetName("test-env")
#if WSL2
                .UseWsl2()
#endif
#if DEBUG
                .AddRabbitMQContainer(p => p with
                {
                    Name = "my-rabbitmq",
                    Reusable = true
                })
#else
                .AddRabbitMQContainer(p => p with
                {
                    Name = "my-rabbitmq"
                })
#endif
                .Build();

            // Act
            await environment.UpAsync();

            // Assert
            var rabbitmq = environment.GetContainer<RabbitMQContainer>("my-rabbitmq");
            PrintServerInformationFromRabbitMq(rabbitmq);
        }

        [Fact]
        public async Task TwoContainersWithSimilarNames_ShouldStartCorrectly()
        {
            // Arrange
#if DEBUG
            var environment = new DockerEnvironmentBuilder(_logger)
#else
            await using var environment = new DockerEnvironmentBuilder(_logger)
#endif
                .SetName("test-env-similar-names")
#if WSL2
                .UseWsl2()
#endif
#if DEBUG
                .AddMongoContainer(p => p with
                {
                    Name = "my-mongo-2",
                    Reusable = true
                })
                .AddMongoContainer(p => p with
                {
                    Name = "my-mongo",
                    Tag = "4.0",
                    Reusable = true
                })
#else
                .AddMongoContainer(p => p with
                {
                    Name = "my-mongo-2"
                })
                .AddMongoContainer(p => p with
                {
                    Name = "my-mongo",
                    Tag = "4.0"
                })
#endif
                .Build();

            // Act
            await environment.UpAsync();

            // Assert
            var mongo = environment.GetContainer<MongoContainer>("my-mongo");
            var mongo2 = environment.GetContainer<MongoContainer>("my-mongo-2");
            PrintMongoVersion(mongo);
            PrintMongoVersion(mongo2);
        }

        [Fact]
        public async Task AddMongoContainer_WhenExecCommand_ShouldPrintMongoVersion()
        {
            // Arrange
            const string userName = "admin";
            const string password = "password123";
#if DEBUG
            var environment = new DockerEnvironmentBuilder(_logger)
#else
            await using var environment = new DockerEnvironmentBuilder(_logger)
#endif
                .SetName("test-env")
#if WSL2
                .UseWsl2()
#endif
#if DEBUG
                .AddMongoContainer(p => p with
                {
                    Name = "my-mongo-3",
                    UserName = userName,
                    Password = password,
                    Reusable = true
                })
#else
                .AddMongoContainer(p => p with
                {
                    Name = "my-mongo-3",
                    UserName = userName,
                    Password = password,
                })
#endif
                .Build();
            await environment.UpAsync();
            var mongo = environment.GetContainer<MongoContainer>("my-mongo-3");

            // Act
            var lsres = await mongo.ExecAsync(new[] { "ls" });
            var whores = await mongo.ExecAsync(new[] { "whoami" });

            // TODO: Fix mongo command
            // var mongoshres = await mongo.ExecAsync(new[] { "mongosh", "--username", userName, "--password", password, "--eval", "\"printjson(db.version())\"" });

            // Assert
            _testOutput.WriteLine($"Exec output whoami: {whores}");
            _testOutput.WriteLine($"Exec output ls: {lsres}");

            // TODO: Fix mongo command
            // _testOutput.WriteLine($"Exec output mongosh: {mongoshres}");
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

        private async Task PrintRedisStats(RedisContainer redis)
        {
            RedisConnectionConfiguration redisConnectionConfiguration = redis.GetConnectionConfiguration();

            var redisConfigurationOptions = new ConfigurationOptions()
            {
                EndPoints =
                {
                    { redisConnectionConfiguration.Host, redisConnectionConfiguration.Port },
                },
                Password = redisConnectionConfiguration.Password,
                AllowAdmin = true
            };

            ConnectionMultiplexer redisC = await ConnectionMultiplexer.ConnectAsync(redisConfigurationOptions);

            _testOutput.WriteLine($"Redis server counters {redisC.GetStatus()}");

            IDatabase database = redisC.GetDatabase();

            string key = "test_key";
            string expectedValue = "test_value";
            await database.StringSetAsync(key, expectedValue);

            string actualValue = await database.StringGetAsync(key);

            Assert.Equal(expectedValue, actualValue);

            await redisC.CloseAsync();
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

        private void PrintServerInformationFromRabbitMq(RabbitMQContainer container)
        {
            var factory = new ConnectionFactory
            {
                HostName = container.Host,
                Port = container.Port,
                UserName = container.UserName,
                Password = container.Password
            };

            using var connection = factory.CreateConnection();

            var infoTextBuilder = new StringBuilder();

            foreach (var serverProperty in connection.ServerProperties)
            {
                if (serverProperty.Value is Dictionary<string, object> dict)
                {
                    infoTextBuilder.AppendLine($"{serverProperty.Key}: {string.Join(", ", dict.Select(x => $"{x.Key}={x.Value}"))}");
                }

                if (serverProperty.Value is byte[] bytes)
                {
                    infoTextBuilder.AppendLine($"{serverProperty.Key}: {Encoding.UTF8.GetString(bytes)}");
                }
            }

            _testOutput.WriteLine($"RabbitMQ server information: {infoTextBuilder}");
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task DisposeEnvironment(DockerEnvironment environment)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
#if !DEBUG
            await environment.DisposeAsync();
#endif
        }
    }
}