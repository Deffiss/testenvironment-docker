namespace TestEnvironment.Docker
{
    internal static class StringExtensions
    {
        public static string GetContainerName(string environmentName, string containerName) =>
            $"{environmentName}-{containerName}";
    }
}
