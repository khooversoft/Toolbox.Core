using System;

namespace Khooversoft.Toolbox.Standard
{
    public class RetryPolicyConfig
    {
        public RetryPolicyConfig(int maxRetry, TimeSpan? withIn)
        {
            maxRetry.VerifyAssert(x => x > 0, "Max retry must be greater then 0");

            MaxRetry = maxRetry;
            WithIn = withIn;
        }

        public int MaxRetry { get; }

        public TimeSpan? WithIn { get; }
    }
}
