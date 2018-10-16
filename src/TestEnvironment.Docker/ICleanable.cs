using System.Threading;
using System.Threading.Tasks;

namespace TestEnvironment.Docker
{
    public interface ICleanable
    {
        /// <summary>
        /// Cleanup the dependency by removing all the data.
        /// </summary>
        /// <param name="token">Cancellation token.</param>
        Task Cleanup(CancellationToken token = default);
    }
}
