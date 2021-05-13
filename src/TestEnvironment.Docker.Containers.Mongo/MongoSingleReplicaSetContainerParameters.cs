namespace TestEnvironment.Docker.Containers.Mongo
{
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
    public record MongoSingleReplicaSetContainerParameters(string Name, string ReplicaSetName)
#pragma warning restore SA1313 // Parameter names should begin with lower-case letter
        : ContainerParameters(Name, "mongo")
    {
        public ushort? CustomReplicaSetPort { get; init; }
    }
}
