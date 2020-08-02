namespace TestEnvironment.Docker.Test.Containers.Database
{
    public abstract class DatabaseContainerBuilder : ContainerBuilder<DatabaseContainer, DatabaseContainerConfiguration>
    {
        public DatabaseContainerBuilder SetDatabaseConfiguration(DatabaseConfiguration databaseConfiguration)
        {
            Configuration.DatabaseConfiguration = databaseConfiguration;

            return this;
        }
    }
}
