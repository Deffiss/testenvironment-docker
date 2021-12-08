namespace TestEnvironment.Docker.Containers.Mail
{
    public record MailContainerParameters(string Name)
       : ContainerParameters(Name, "mailhog/mailhog")
    {
        public ushort SmptPort { get; init; } = 1025;

        public ushort ApiPort { get; init; } = 8025;

        public string DeleteEndpoint { get; init; } = "api/v1/messages";
    }
}
