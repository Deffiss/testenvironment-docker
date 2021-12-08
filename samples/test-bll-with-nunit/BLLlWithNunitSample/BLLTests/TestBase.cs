using System.Threading.Tasks;
using DAL;
using NUnit.Framework;
using TestEnvironment.Docker;
using TestEnvironment.Docker.Containers.Mssql;

namespace BLLTests
{
    public class TestBase
    {
        private IDockerEnvironment _dockerEnvironment;

        protected PizzaContext DbContext { get; set; }

        [OneTimeSetUp]
        public async Task OneTimeSetup()
        {
            var environmentBuilder = new DockerEnvironmentBuilder();
            _dockerEnvironment = PrepareDockerEnvironment(environmentBuilder);
            await _dockerEnvironment.UpAsync();

            var connectionString = _dockerEnvironment.GetContainer<MssqlContainer>("testDb").GetConnectionString();

            DbContext = new PizzaContext(connectionString);
            await DbContext.Database.EnsureCreatedAsync();
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            await _dockerEnvironment.DisposeAsync();
            await DbContext.DisposeAsync();
        }

        [TearDown]
        public async Task ClearDb()
        {
            DbContext.PizzaOrders.RemoveRange(DbContext.PizzaOrders);
            DbContext.Pizzas.RemoveRange(DbContext.Pizzas);
            DbContext.Orders.RemoveRange(DbContext.Orders);
            await DbContext.SaveChangesAsync();
        }

        private IDockerEnvironment PrepareDockerEnvironment(DockerEnvironmentBuilder environmentBuilder)
        {
            return environmentBuilder
                .SetName("nunit-tests")
                .AddMssqlContainer(p => p with
                {
                    Name = "testDb",
                    SAPassword = "HelloK11tt_0"
                })
                .Build();
        }
    }
}
