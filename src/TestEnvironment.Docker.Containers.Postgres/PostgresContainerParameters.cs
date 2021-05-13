namespace TestEnvironment.Docker.Containers.Postgres
{
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
    public record PostgresContainerParameters(string Name, string UserName, string Password)
#pragma warning restore SA1313 // Parameter names should begin with lower-case letter
        : ContainerParameters(Name, "postgres")
    {
    }
}
