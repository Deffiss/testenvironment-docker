using MongoDB.Driver;
using Nest;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TestEnvironment.Docker.Containers.Elasticsearch;
using TestEnvironment.Docker.Containers.Mongo;
using TestEnvironment.Docker.Containers.Mssql;
using Xunit;

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
                .AddElasticsearchContainer("my-elastic", reuseContainer: true)
                .AddMssqlContainer("my-mssql", "HelloK11tt_0", environmentVariables: new Dictionary<string, string> { ["MSSQL_COLLATION"] = "SQL_Latin1_General_CP1_CS_AS" }, reuseContainer: true)
                .AddMongoContainer("my-mongo", reuseContainer: true)
                .AddFromDockerfile("from-file", "Dockerfile", containerWaiter: new HttpContainerWaiter("/", httpPort: 8080), reuseContainer: true)
#else
                .AddElasticsearchContainer("my-elastic")
                .AddMssqlContainer("my-mssql", "HelloK11tt_0")
                .AddMongoContainer("my-mongo")
                .AddFromDockerfile("from-file", "Dockerfile", containerWaiter: new HttpContainerWaiter("/", httpPort: 8080))
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

#if !DEBUG
            // Down it.
            await environment.Down();

            // Dispose (remove).
            environment.Dispose();
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
    }
}
