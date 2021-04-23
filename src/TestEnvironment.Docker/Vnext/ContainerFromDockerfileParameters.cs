namespace TestEnvironment.Docker.Vnext
{
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
    public record ContainerFromDockerfileParameters(string Name, string ImageName)
        : ContainerParameters(Name, ImageName)
#pragma warning restore SA1313 // Parameter names should begin with lower-case letter
    {
    }
}
