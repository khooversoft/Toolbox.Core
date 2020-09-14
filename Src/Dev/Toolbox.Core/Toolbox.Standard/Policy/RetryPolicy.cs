using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Khooversoft.Toolbox.Standard
{
    public class RetryPolicy
    {
        private object _lock = new object();

        private readonly TimeSpan _maxTime;
        private DateTime? _lastRetryDate;

        public RetryPolicy(TimeSpan maxTime) => _maxTime = maxTime;

        public void Reset()
        {
            lock (_lock)
            {
                _lastRetryDate = null;
            }
        }

        public bool CanRetry()
        {
            lock (_lock)
            {
                _lastRetryDate ??= DateTime.Now;
            }

            return DateTime.Now - _lastRetryDate <= _maxTime;
        }
    }
}
