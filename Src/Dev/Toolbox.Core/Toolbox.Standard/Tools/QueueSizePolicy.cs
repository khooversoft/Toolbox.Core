using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Khooversoft.Toolbox.Standard
{
    public interface IQueueSizePolicy<T> : IEnumerable<T>
    {
        int MaxSize { get; }
        int Count { get; }
        bool IsFull { get; }
        int LostCount { get; }

        void Clear();
        void Enqueue(T value);
        T Dequeue();
        T Peek();
    }

    public class QueueSizePolicy<T> : IQueueSizePolicy<T>
    {
        private readonly Queue<T> _queue;
        private int _lostCount;
        private readonly object _lock = new object();

        public QueueSizePolicy(Queue<T> queue, int maxSize)
        {
            queue.Verify().IsNotNull();
            _queue = queue;
            MaxSize = maxSize;
        }

        public int MaxSize { get; }

        public int Count => _queue.Count;

        public int LostCount => _lostCount;

        public bool IsFull => Count == MaxSize;

        public void Clear()
        {
            lock (_lock)
            {
                _queue.Clear();
            }
        }

        public void Enqueue(T value)
        {
            lock(_lock)
            {
                if (_queue.Count >= MaxSize)
                {
                    _queue.Dequeue();
                    _lostCount++;
                }

                _queue.Enqueue(value);
            }
        }

        public T Dequeue()
        {
            lock(_lock)
            {
                return _queue.Dequeue();
            }
        }

        public T Peek()
        {
            lock (_lock)
            {
                return _queue.Peek();
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            lock (_lock)
            {
                return _queue.ToList().GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public static class QueueSizePolicyExtensions
    {
        public static IQueueSizePolicy<T> SetFixSizePolicy<T>(this Queue<T> queue, int maxSize)
        {
            return new QueueSizePolicy<T>(queue, maxSize);
        }
    }
}
