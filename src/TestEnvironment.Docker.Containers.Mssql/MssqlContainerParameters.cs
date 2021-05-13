namespace TestEnvironment.Docker.Containers.Mssql
{
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
    public record MssqlContainerParameters(string Name, string SAPassword)
#pragma warning restore SA1313 // Parameter names should begin with lower-case letter
        : ContainerParameters(Name, "mcr.microsoft.com/mssql/server")
    {
    }
}
