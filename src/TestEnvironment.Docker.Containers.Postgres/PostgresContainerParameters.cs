namespace TestEnvironment.Docker.Containers.Postgres
{
    public record PostgresContainerParameters(string Name, string UserName, string Password)
        : ContainerParameters(Name, "postgres")
    {
    }
}
