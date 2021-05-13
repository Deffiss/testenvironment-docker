namespace TestEnvironment.Docker.Containers.Mongo
{
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
    public record MongoContainerParameters(string Name, string UserName, string Password)
#pragma warning restore SA1313 // Parameter names should begin with lower-case letter
        : ContainerParameters(Name, "mongo")
    {
    }
}
