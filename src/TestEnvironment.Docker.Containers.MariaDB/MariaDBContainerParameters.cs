namespace TestEnvironment.Docker.Containers.MariaDB
{
    public record MariaDBContainerParameters(string Name, string RootPassword)
        : ContainerParameters(Name, "mariadb")
    {
    }
}
