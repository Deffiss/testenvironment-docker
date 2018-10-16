using Nest;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TestEnvironment.Docker.Containers;
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
                .SetName("my-test-env")
                .AddContainer("my-nginx", "nginx")
                .AddElasticsearchContainer("my-elastic")
                .AddMssqlContainer("my-mssql", "HelloK11tt_0")
                .Build();

            // Up it.
            await environment.Up();

            // Play with containers.
            var mssql = environment.GetContainer<MssqlContainer>("my-mssql");
            await PrintMssqlVersion(mssql);

            var elastic = environment.GetContainer<ElasticsearchContainer>("my-elastic");
            await PrintElasticsearchVersion(elastic);

            // Down it.
            await environment.Down();

            // Dispose (remove).
            environment.Dispose();
        }

        private static async Task PrintMssqlVersion(MssqlContainer mssql)
        {
            using (var command = new SqlCommand("SELECT @@VERSION", new SqlConnection(mssql.GetConnectionString())))
            {
                command.Connection.Open();

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
    }
}
