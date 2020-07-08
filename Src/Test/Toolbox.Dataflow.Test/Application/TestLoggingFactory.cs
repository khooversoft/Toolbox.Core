using Microsoft.Extensions.Logging;
using System;
using Xunit.Abstractions;

namespace Toolbox.Dataflow.Test.Application
{
    internal class TestLoggingFactory
    {
        private readonly ILoggerFactory _factory;

        public TestLoggingFactory(ITestOutputHelper testOutput)
        {
            _factory = LoggerFactory.Create(builder =>
            {
                builder.AddDebug();
                builder.AddFilter(x => true);
                builder.AddProvider(new XUnitLoggerProvider(testOutput));
            });
        }

        public ILogger<T> CreateLogger<T>() => _factory.CreateLogger<T>();

        private class XUnitLoggerProvider : ILoggerProvider
        {
            private readonly ITestOutputHelper _testOutputHelper;

            public XUnitLoggerProvider(ITestOutputHelper testOutputHelper)
            {
                _testOutputHelper = testOutputHelper;
            }

            public ILogger CreateLogger(string categoryName) => new XUnitLogger(_testOutputHelper, categoryName);

            public void Dispose() => throw new NotImplementedException();
        }

        private class XUnitLogger : ILogger
        {
            private readonly ITestOutputHelper _testOutputHelper;
            private readonly string _name;

            public XUnitLogger(ITestOutputHelper testOutputHelper, string name)
            {
                _testOutputHelper = testOutputHelper;
                _name = name;
            }

            public IDisposable BeginScope<TState>(TState state) => null!;

            public bool IsEnabled(LogLevel logLevel) => true;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                _testOutputHelper.WriteLine($"[{_name}]: " + formatter(state, exception));
            }
        }
    }
}
