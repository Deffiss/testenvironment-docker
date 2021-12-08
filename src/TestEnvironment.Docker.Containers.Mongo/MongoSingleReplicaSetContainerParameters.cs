namespace TestEnvironment.Docker.Containers.Mongo
{
    public record MongoSingleReplicaSetContainerParameters(string Name, string ReplicaSetName)
        : ContainerParameters(Name, "mongo")
    {
        public ushort? CustomReplicaSetPort { get; init; }
    }
}
