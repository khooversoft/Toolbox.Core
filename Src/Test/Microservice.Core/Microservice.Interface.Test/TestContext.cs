//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;

//namespace Microservice.Interface.Test
//{
//    public class TestContext : ITestContext
//    {
//        private int _messageCount;
//        private readonly List<object> _messages = new List<object>();
//        private readonly object _lock = new object();

//        public IReadOnlyList<object> Messages
//        {
//            get
//            {
//                lock (_lock)
//                {
//                    return _messages.ToList();
//                }
//            }
//        }

//        public int MessageCount => _messageCount;

//        public void AddMessage(object message)
//        {
//            Interlocked.Increment(ref _messageCount);
//            lock (_lock)
//            {
//                _messages.Add(message);
//            }
//        }

//        public void IncrementMessageCount()
//        {
//            Interlocked.Increment(ref _messageCount);
//        }
//    }
//}
