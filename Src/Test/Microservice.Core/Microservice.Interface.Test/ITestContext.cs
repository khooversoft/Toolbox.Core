using System;
using System.Collections.Generic;
using System.Text;

namespace Microservice.Interface.Test
{
    public interface ITestContext
    {
        int MessageCount { get; }

        IReadOnlyList<string> Messages { get; }

        void IncrementMessageCount();

        void AddMessageAsString(string message);
    }
}
