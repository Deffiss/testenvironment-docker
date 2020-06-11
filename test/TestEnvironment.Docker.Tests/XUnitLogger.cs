using System;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace TestEnvironment.Docker.Tests
{
    public class XUnitLogger : ILogger
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public XUnitLogger(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            var message = formatter(state, exception);

            _testOutputHelper.WriteLine($"{logLevel.ToString()}: {message}");
        }

        public bool IsEnabled(LogLevel logLevel) => true;

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }
    }
}
