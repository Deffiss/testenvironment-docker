namespace TestEnvironment.Docker.Test.Helpers
{
    public static class StringExtensions
    {
        public static string GetContainerName(this string containerName, string environmentName) =>
            $"{environmentName}-{containerName}";
    }
}
