namespace TestEnvironment.Docker.Containers.Mssql
{
    public record MssqlContainerParameters(string Name, string SAPassword)
        : ContainerParameters(Name, "mcr.microsoft.com/mssql/server")
    {
    }
}
