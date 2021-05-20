namespace TestEnvironment.Docker.Containers.Mongo
{
    public record MongoContainerParameters(string Name, string UserName, string Password)
        : ContainerParameters(Name, "mongo")
    {
    }
}
