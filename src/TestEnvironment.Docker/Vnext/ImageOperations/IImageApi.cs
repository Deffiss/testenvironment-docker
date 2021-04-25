using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TestEnvironment.Docker.Vnext.ImageOperations
{
    public interface IImageApi
    {
        Task PullImageAsync(string imageName, string tag, CancellationToken cancellationToken = default);

        Task BuildImageAsync(string dockerfile, string imageName, string tag = "latest", string context = ".", IDictionary<string, string>? buildArgs = default, CancellationToken cancellationToken = default);
    }
}
