using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Standard
{
    public class RetryPolicy
    {
        public RetryPolicy(TimeSpan timeout, TimeSpan delay)
        {
            delay.Verify(nameof(delay)).IsNotNull();

            Timeout = timeout;
            Delay = delay;
        }

        public TimeSpan Timeout { get; }

        public TimeSpan Delay { get; }
    }

    public static class RetryPolicyExtensions
    {
        public static async Task Execute(this RetryPolicy retryPolicy, Func<Task> function)
        {
            function.Verify(nameof(function)).IsNotNull();

            var startTime = DateTime.Now;

            while (true)
            {
                try
                {
                    await function();
                    return;
                }
                catch (Exception ex)
                {
                    if (DateTime.Now - startTime > retryPolicy.Timeout) throw new TimeoutException("Retry policy failed", ex);
                }

                await Task.Delay(retryPolicy.Delay);
            }
        }
    }
}
