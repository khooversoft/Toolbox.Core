using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Microservice.Interface.Test
{
    public class TestContext : ITestContext
    {
        private int _messageCount;
        private readonly List<string> _messages = new List<string>();
        private readonly object _lock = new object();

        public IReadOnlyList<string> Messages
        {
            get
            {
                lock (_lock)
                {
                    return _messages.ToList();
                }
            }
        }

        public int MessageCount => _messageCount;

        public void AddMessageAsString(string message)
        {
            Interlocked.Increment(ref _messageCount);
            lock(_lock)
            {
                _messages.Add(message);
            }
        }

        public void IncrementMessageCount()
        {
            Interlocked.Increment(ref _messageCount);
        }
    }
}
