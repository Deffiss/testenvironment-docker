namespace TestEnvironment.Docker.Test.Environments
{
    public abstract class BaseEnvironmentBuilder<TEnvironment, TEnvironmentConfiguration>
    {
        public abstract TEnvironment Build();
    }
}
