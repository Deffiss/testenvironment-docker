using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace TestEnvironment.Docker
{
    public abstract class BaseContainerWaiter<TContainer> : IContainerWaiter<TContainer>
        where TContainer : Container
    {
        protected ILogger Logger { get; }

        protected virtual int AttemptsCount => 60;
        protected virtual TimeSpan DelayTime => TimeSpan.FromSeconds(1);

        protected BaseContainerWaiter(ILogger logger)
        {
            Logger = logger;
        }

        public async Task<bool> Wait(TContainer container, CancellationToken cancellationToken)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            var attempts = AttemptsCount;
            do
            {
                var isAlive = await PerformCheck(container, cancellationToken);

                if (isAlive) return true;

                attempts--;
                await Task.Delay(DelayTime, cancellationToken);
            } while (attempts != 0);

            Logger.LogError($"Container {container.Name} didn't start.");
            return false;
        }

        protected abstract Task<bool> PerformCheck(TContainer container, CancellationToken cancellationToken);

        public Task<bool> Wait(Container container, CancellationToken cancellationToken) =>
            Wait(container as TContainer, cancellationToken);
    }
}