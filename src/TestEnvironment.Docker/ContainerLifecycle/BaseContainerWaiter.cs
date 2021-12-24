using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace TestEnvironment.Docker.ContainerLifecycle
{
    public abstract class BaseContainerWaiter<TContainer> : IContainerWaiter<TContainer>
        where TContainer : Container
    {
        protected ILogger? Logger { get; init; }

        protected virtual int AttemptsCount => 60;

        protected virtual TimeSpan DelayTime => TimeSpan.FromSeconds(1);

#pragma warning disable SA1201 // Elements should appear in the correct order
        public BaseContainerWaiter()
#pragma warning restore SA1201 // Elements should appear in the correct order
        {
        }

        public BaseContainerWaiter(ILogger logger) =>
            Logger = logger;

        public async Task<bool> WaitAsync(TContainer container, CancellationToken cancellationToken)
        {
            Logger?.LogInformation($"{container.Name}: checking container state...");

            var policy = PolicyFactory.CreateWaitPolicy(AttemptsCount, DelayTime, IsRetryable, e => Logger?.LogError(e, $"{container.Name} check failed with exception {e.Message}"));
            var result = await policy.ExecuteAndCaptureAsync(async () => await PerformCheckAsync(container, cancellationToken));

            if (result.Outcome == Polly.OutcomeType.Successful)
            {
                Logger?.LogInformation($"{container.Name}: container is Up!");
            }
            else
            {
                Logger?.LogError($"Container {container.Name} didn't start.");
            }

            return result.Result;
        }

        public Task<bool> WaitAsync(Container container, CancellationToken cancellationToken) =>
            WaitAsync((TContainer)container, cancellationToken);

        protected abstract Task<bool> PerformCheckAsync(TContainer container, CancellationToken cancellationToken);

        protected virtual bool IsRetryable(Exception exception) => true;
    }
}
