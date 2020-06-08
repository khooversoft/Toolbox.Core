using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Toolbox.Standard
{
    public class MemoryLogger : ILogger
    {
        private const int _maxSize = 100;
        private readonly ConcurrentQueue<string> _queue = new ConcurrentQueue<string>();

        public IDisposable BeginScope<TState>(TState state) => null!;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            _queue.Enqueue(formatter(state, exception));

            while (_queue.Count > _maxSize) _queue.TryDequeue(out string _);
        }

        public IReadOnlyList<string> LoggedItems => _queue.ToArray();

        public ILogger<T> CreateLogger<T>() => new MemoryLoggerStub<T>();

        private class MemoryLoggerStub<T> : MemoryLogger, ILogger<T>
        {
        }
    }
}
