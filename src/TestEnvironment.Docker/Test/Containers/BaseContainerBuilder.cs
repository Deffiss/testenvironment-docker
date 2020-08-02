namespace TestEnvironment.Docker.Test.Containers
{
    public abstract class BaseContainerBuilder<TContainer, TConfiguration>
    {
        public abstract TContainer Build();
    }
}
