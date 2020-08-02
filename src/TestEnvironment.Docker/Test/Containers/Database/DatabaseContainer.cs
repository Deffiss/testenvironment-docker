using TestEnvironment.Docker.Test.Containers.Database;

namespace TestEnvironment.Docker.Test.Containers
{
    public abstract class DatabaseContainer : Container<DatabaseContainerConfiguration>
    {
        public DatabaseContainer(DatabaseContainerConfiguration configuration)
            : base(configuration)
        {
        }

        public static new DatabaseContainerBuilder Create()
        {
            return new DatabaseContainerBuilder();
        }

        public abstract string ConnectionString();
    }
}
