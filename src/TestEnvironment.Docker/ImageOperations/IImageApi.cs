using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TestEnvironment.Docker.ImageOperations
{
    public interface IImageApi
    {
        Task PullImageAsync(string imageName, string tag, CancellationToken cancellationToken = default);

#pragma warning disable SA1011 // Closing square brackets should be spaced correctly
        Task BuildImageAsync(string dockerfile, string imageName, string tag = "latest", string context = ".", IDictionary<string, string>? buildArgs = default, string[]? ignoredFiles = default, CancellationToken cancellationToken = default);
#pragma warning restore SA1011 // Closing square brackets should be spaced correctly
    }
}
