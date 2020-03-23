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

namespace TestEnvironment.Docker.Tests
{
    public class DockerEnvironmentTests
    {
        [Fact]
        public async Task CreateDockerEnvironment()
        {
            // Create the environment using builder pattern
            var environment = new DockerEnvironmentBuilder()
                .UseDefaultNetwork()
                .SetName("test-env")
                .AddContainer("my-nginx", "nginx")
#if DEBUG
                .AddElasticsearchContainer("my-elastic", ports: new Dictionary<ushort, ushort> { [9200] = 9200 }, reuseContainer: true)
                .AddMssqlContainer("my-mssql", "HelloK11tt_0", environmentVariables: new Dictionary<string, string> { ["MSSQL_COLLATION"] = "SQL_Latin1_General_CP1_CS_AS" }, reuseContainer: true)
                .AddMariaDBContainer("my-maria", "my-secret-pw", reuseContainer: true)
                .AddMongoContainer("my-mongo", reuseContainer: true)
                .AddMailContainer("my-mail", reuseContainer: true)
                .AddFtpContainer("my-ftp", "superuser", "test", ports: Enumerable.Range(30000, 10).ToDictionary(p => (ushort)p, p => (ushort)p).MergeDictionaries(new Dictionary<ushort, ushort> { [21] = 21 }), reuseContainer: true)
                .AddFromDockerfile("from-file", "Dockerfile", containerWaiter: new HttpContainerWaiter("/", httpPort: 8080), reuseContainer: true)
                .AddPostgresContainer("my-postgres", reuseContainer: true)
                ////.AddContainerDependency(fromContainerName: "my-postgres", toContainerName: "my-mongo")
                ////.AddContainerDependency(fromContainerName: "my-mongo", toContainerName: "my-postgres")
#else
                .AddElasticsearchContainer("my-elastic")
                .AddMssqlContainer("my-mssql", "HelloK11tt_0")
                .AddMariaDBContainer("my-maria", "my-secret-pw")
                .AddMongoContainer("my-mongo")
                .AddMailContainer("my-mail")
                .AddFromDockerfile("from-file", "Dockerfile", containerWaiter: new HttpContainerWaiter("/", httpPort: 8080))
                .AddFtpContainer("my-ftp", "superuser", "test", ports: Enumerable.Range(30000, 10).ToDictionary(p => (ushort)p, p => (ushort)p).MergeDictionaries(new Dictionary<ushort, ushort> { [21] = 21 }))
                .AddPostgresContainer("my-postgres")

#endif
                .Build();

            // Up it.
            await environment.Up();

            // Play with containers.
            var mssql = environment.GetContainer<MssqlContainer>("my-mssql");
            await PrintMssqlVersion(mssql);

            var elastic = environment.GetContainer<ElasticsearchContainer>("my-elastic");
            await PrintElasticsearchVersion(elastic);

            var mongo = environment.GetContainer<MongoContainer>("my-mongo");
            PrintMongoVersion(mongo);

            var staticFilesContainer = environment.GetContainer("from-file");
            await PrintReturnedHtml(staticFilesContainer);

            var maria = environment.GetContainer<MariaDBContainer>("my-maria");
            await PrintMariaDBVersion(maria);

            var ftp = environment.GetContainer<FtpContainer>("my-ftp");
            await PrintFtpServerType(ftp);

            var mail = environment.GetContainer<MailContainer>("my-mail");
            await PrintSmtpCapabilities(mail);

            var postgres = environment.GetContainer<PostgresContainer>("my-postgres");
            await PrintPostgresDbVersion(postgres);

#if !DEBUG
            // Down it.
            await environment.Down();

            // Dispose (remove).
            await environment.DisposeAsync();
#endif
        }

        private static async Task PrintMssqlVersion(MssqlContainer mssql)
        {
            using (var connection = new SqlConnection(mssql.GetConnectionString()))
            using (var command = new SqlCommand("SELECT @@VERSION", connection))
            {
                await connection.OpenAsync();

                var reader = await command.ExecuteReaderAsync();
                await reader.ReadAsync();

                Console.WriteLine($"MSSQL Version: {reader.GetString(0)}");
            }
        }

        private static async Task PrintElasticsearchVersion(ElasticsearchContainer elastic)
        {
            var elasticClient = new ElasticClient(new Uri(elastic.GetUrl()));
            var clusterInfo = await elasticClient.NodesInfoAsync();
            Console.WriteLine($"Elasticsearch version: {clusterInfo.Nodes.Values.First().Version}");
        }

        private static void PrintMongoVersion(MongoContainer mongo)
        {
            var mongoClient = new MongoClient(mongo.GetConnectionString());
            var clusterDescription = mongoClient.Cluster.Description;
            Console.WriteLine($"Mongo version: {clusterDescription.Servers.First().Version}");
        }

        private static async Task PrintMariaDBVersion(MariaDBContainer maria)
        {
            using (var connection = new MySqlConnection(maria.GetConnectionString()))
            using (var command = new MySqlCommand("select @@version", connection))
            {
                await command.Connection.OpenAsync();

                var reader = await command.ExecuteReaderAsync();
                await reader.ReadAsync();

                Console.WriteLine($"MariaDB Version: {reader.GetString(0)}");
            }
        }

        private static async Task PrintFtpServerType(FtpContainer ftpContainer)
        {
            using (var ftpClient = new FtpClient(ftpContainer.FtpHost, ftpContainer.IsDockerInDocker ? 21 : ftpContainer.Ports[21], ftpContainer.FtpUserName, ftpContainer.FtpPassword))
            {
                await ftpClient.ConnectAsync();

                Console.WriteLine($"FTP type: {ftpClient.ServerType}");
            }
        }

        private static async Task PrintSmtpCapabilities(MailContainer mailContainer)
        {
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(mailContainer.IsDockerInDocker ? mailContainer.IPAddress : "localhost", mailContainer.IsDockerInDocker ? 1025 : mailContainer.Ports[1025]);

                Console.WriteLine($"Smtp capabilites: {client.Capabilities}");
            }
        }

        private static async Task PrintReturnedHtml(Container staticFilesContainer)
        {
            var uri = new Uri($"http://{(staticFilesContainer.IsDockerInDocker ? staticFilesContainer.IPAddress : "localhost")}:" +
                $"{(staticFilesContainer.IsDockerInDocker ? 8080 : staticFilesContainer.Ports[8080])}");

            using (var client = new HttpClient { BaseAddress = uri })
            {
                var response = await client.GetStringAsync("/");
                Console.WriteLine($"Response from static server: {response}");
            }
        }

        private static async Task PrintPostgresDbVersion(PostgresContainer postgres)
        {
            using (var connection = new NpgsqlConnection(postgres.GetConnectionString()))
            using (var command = new NpgsqlCommand("select version();", connection))
            {
                await connection.OpenAsync();

                var reader = await command.ExecuteReaderAsync();
                await reader.ReadAsync();

                Console.WriteLine($"Postgres Version: {reader.GetString(0)}");
            }
        }
    }
}
