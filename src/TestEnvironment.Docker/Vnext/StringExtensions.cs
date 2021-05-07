namespace TestEnvironment.Docker.Vnext
{
    internal static class StringExtensions
    {
        public static string GetContainerName(string environmentName, string containerName) =>
            $"{environmentName}-{containerName}";
    }
}
