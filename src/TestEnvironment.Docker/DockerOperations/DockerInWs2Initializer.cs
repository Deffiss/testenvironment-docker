using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet;
using Microsoft.Extensions.Logging;

namespace TestEnvironment.Docker.DockerOperations
{
    public class DockerInWs2Initializer : IDockerInitializer
    {
        private const string DockerServiceInitCommand = @"-u root -e sh -c ""service docker status || service docker start""";
        private const string Wsl2Executable = @"wsl.exe";
        private const int DockerServicePollRetryCount = 30;
        private const int DockerServicePollRetryTimeout = 1;

        private readonly IDockerClient _dockerClient;
        private readonly ILogger? _logger;

        private Process? _wsl2ShellProcess;

        public DockerInWs2Initializer(IDockerClient dockerClient, ILogger? logger) =>
            (_dockerClient, _logger) = (dockerClient, logger);

        public async Task InitializeDockerAsync(CancellationToken cancellationToken = default)
        {
            if (_wsl2ShellProcess is not null)
            {
                return;
            }

            var dockerServiceStarted = false;
            Process? wsl2ShellProcess = null;

            try
            {
                wsl2ShellProcess = new()
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = Wsl2Executable,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardInput = true,
                        CreateNoWindow = true,
                        Arguments = DockerServiceInitCommand
                    }
                };

                wsl2ShellProcess.Start();

                dockerServiceStarted = await WaitForDockerReadinessAsync(cancellationToken);

                if (dockerServiceStarted)
                {
                    _wsl2ShellProcess = wsl2ShellProcess;
                }
            }
            finally
            {
                if (!dockerServiceStarted)
                {
                    wsl2ShellProcess?.Dispose();
                }
            }

            if (!dockerServiceStarted)
            {
                throw new InvalidOperationException("Unable to run Docker in WSL.");
            }
        }

        public ValueTask DisposeAsync()
        {
            // TODO: maybe stop Docker service
            _wsl2ShellProcess?.Dispose();
            return ValueTask.CompletedTask;
        }

        private async Task<bool> WaitForDockerReadinessAsync(CancellationToken cancellationToken = default)
        {
            _logger?.LogInformation("Checking for docker service state...");

            var policy = PolicyFactory.CreateWaitPolicy(DockerServicePollRetryCount, TimeSpan.FromSeconds(DockerServicePollRetryTimeout), null, e => _logger?.LogError(e, "Docker service is not available."));
            var result = await policy.ExecuteAndCaptureAsync(async () =>
            {
                await _dockerClient.System.PingAsync(cancellationToken);
                return true;
            });

            if (result.Outcome == Polly.OutcomeType.Successful)
            {
                _logger?.LogInformation("Docker service is up!");
            }
            else
            {
                _logger?.LogError("Docker service didn't start.");
            }

            return result.Result;
        }
    }
}
