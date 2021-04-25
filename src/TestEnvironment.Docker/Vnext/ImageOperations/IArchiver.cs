using System.Threading;
using System.Threading.Tasks;

namespace TestEnvironment.Docker.Vnext.ImageOperations
{
    public interface IArchiver
    {
        Task CreateTarArchiveAsync(string fileName, string directiry, CancellationToken token = default);
    }
}
