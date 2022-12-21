namespace TestEnvironment.Docker.Containers.RabbitMQ
{
    public record RabbitMQContainerParameters(string Name, string UserName, string Password)
        : ContainerParameters(Name, "rabbitmq");
}