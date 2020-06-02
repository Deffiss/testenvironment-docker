using System.Threading.Tasks;
using DAL;
using NUnit.Framework;
using TestEnvironment.Docker;
using TestEnvironment.Docker.Containers.Mssql;

namespace DALTests
{
    public class TestBase
    {
        private DockerEnvironment _dockerEnvironment;

        protected PizzaContext DbContext { get; set; }

        [OneTimeSetUp]
        public async Task OneTimeSetup()
        {
            var environmentBuilder = new DockerEnvironmentBuilder();
            _dockerEnvironment = PrepareDockerEnvironment(environmentBuilder);
            await _dockerEnvironment.Up();

            var connectionString = _dockerEnvironment.GetContainer<MssqlContainer>("testDb").GetConnectionString();

            DbContext = new PizzaContext(connectionString);
            await DbContext.Database.EnsureCreatedAsync();
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            await _dockerEnvironment.DisposeAsync();
        }

        [TearDown]
        public async Task ClearDb()
        {
            DbContext.Pizzas.RemoveRange(DbContext.Pizzas);
            await DbContext.SaveChangesAsync();
        }

        private DockerEnvironment PrepareDockerEnvironment(DockerEnvironmentBuilder environmentBuilder)
        {
            return environmentBuilder.UseDefaultNetwork().SetName("nunit-tests").AddMssqlContainer("testDb", "HelloK11tt_0").Build();
        }
    }
}
