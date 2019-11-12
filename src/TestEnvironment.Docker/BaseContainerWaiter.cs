using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace TestEnvironment.Docker
{
    public abstract class BaseContainerWaiter<TContainer> : IContainerWaiter<TContainer>
        where TContainer : Container
    {
        private const int AttemptsCount = 60;
        private const int DelayTime = 1000;

        private readonly ILogger _logger;

        protected BaseContainerWaiter(ILogger logger)
        {
            _logger = logger;
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

            _logger.LogError($"Container {container.Name} didn't start.");
            return false;
        }

        protected abstract Task<bool> PerformCheck(TContainer container, CancellationToken cancellationToken);

        public Task<bool> Wait(Container container, CancellationToken cancellationToken) =>
            Wait(container as TContainer, cancellationToken);
    }
}