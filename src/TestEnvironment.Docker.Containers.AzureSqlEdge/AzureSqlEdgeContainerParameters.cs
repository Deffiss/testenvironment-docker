namespace TestEnvironment.Docker.Containers.AzureSqlEdge
{
    public record AzureSqlEdgeContainerParameters(string Name, string SAPassword)
        : ContainerParameters(Name, "mcr.microsoft.com/azure-sql-edge");
}