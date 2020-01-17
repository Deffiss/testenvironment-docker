using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TestEnvironment.Docker;
using TestEnvironment.Docker.Containers.Mongo;
using Xunit;

namespace AspNetCoreSample.Tests
{
    public class EnvironmentFixture : IAsyncLifetime
    {
        private readonly string _environmentName;
        private DockerEnvironment _environment;
        private IHost _host;

        public HttpClient TestClient { get; private set; }

        public EnvironmentFixture(string environmentName)
        {
            _environmentName = environmentName;
        }

        public async Task InitializeAsync()
        {
            // Docker environment seutup.
            _environment = CreateTestEnvironmentBuilder().Build();
            await _environment.Up();

            // API Test host setup
            var mongoContainer = _environment.GetContainer<MongoContainer>("mongo");
            _host = await CreateHostBuilder(mongoContainer).StartAsync();
            TestClient = _host.GetTestClient();

            await OnInitialized(mongoContainer);
        }

        public async Task DisposeAsync()
        {
            TestClient.Dispose();
            _host.Dispose();

#if !DEBUG
            await _environment.DisposeAsync();
#endif
        }

        protected virtual Task OnInitialized(MongoContainer mongoContainer) => Task.CompletedTask;

        private IDockerEnvironmentBuilder CreateTestEnvironmentBuilder() =>
            new DockerEnvironmentBuilder()
                .SetName(_environmentName)
#if DEBUG
                .AddMongoContainer("mongo", reuseContainer: true);
#else
                .AddMongoContainer("mongo");
#endif

        private IHostBuilder CreateHostBuilder(MongoContainer mongoContainer)
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new[] {
                    new KeyValuePair<string, string>("MongoDbConnectionString", mongoContainer.GetConnectionString())
                });

            var builder = new HostBuilder()
                .ConfigureWebHost(webBuilder =>
                {
                    webBuilder
                        .UseStartup<Startup>()
                        .UseConfiguration(config.Build())
                        .UseTestServer();
                });

            return builder;
        }
    }
}
