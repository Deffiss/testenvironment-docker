using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TestEnvironment.Docker
{
    /// <summary>
    /// Represents test environment dependency.
    /// </summary>
    public interface IDependency : IDisposable
    {
        /// <summary>
        /// Gets the dependency name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Run the dependency.
        /// </summary>
        /// <param name="token">Cancellation token.</param>
        Task Run(IDictionary<string, string> environmentVariables, CancellationToken token = default);

        /// <summary>
        /// Stop the dependency.
        /// </summary>
        /// <param name="token">Cancellation token.</param>
        Task Stop(CancellationToken token = default);
    }
}
