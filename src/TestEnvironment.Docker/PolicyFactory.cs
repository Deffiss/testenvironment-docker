using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Polly;
using Polly.Retry;

namespace TestEnvironment.Docker
{
    public static class PolicyFactory
    {
        public static AsyncRetryPolicy<bool> CreateWaitPolicy(int retryCount, TimeSpan retryInterval, Func<Exception, bool>? exceptionFilter, Action<Exception>? onRetry)
        {
            var exceptionPb = exceptionFilter is null
                ? Policy.Handle<Exception>()
                : Policy.Handle<Exception>(e => exceptionFilter(e));

            var withResultPb = exceptionPb.OrResult<bool>(res => !res); // if returned false from healthcheck retry as well

            return onRetry is null
                ? withResultPb.WaitAndRetryAsync(retryCount, i => retryInterval)
                : withResultPb.WaitAndRetryAsync(retryCount, i => retryInterval, (e, ts) =>
                {
                    if (e.Exception is not null)
                    {
                        onRetry.Invoke(e.Exception);
                    }
                });
        }
    }
}
