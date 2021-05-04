using System.Threading;
using System.Threading.Tasks;

namespace TestEnvironment.Docker.Vnext.ImageOperations
{
    public interface IArchiver
    {
#pragma warning disable SA1011 // Closing square brackets should be spaced correctly
        Task CreateTarArchiveAsync(string fileName, string directiry, string[]? ignoredFiles = default, CancellationToken token = default);
#pragma warning restore SA1011 // Closing square brackets should be spaced correctly
    }
}
