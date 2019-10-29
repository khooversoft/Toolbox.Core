using System;
using System.Collections.Generic;
using System.Text;
using Toolbox.Core.Extensions.Configuration;

namespace EventHubPerformanceTest
{
    public class EventHub
    {
        [Option("Event hub connection string")]
        public string ConnectionString { get; set; }

        [Option("Event hub name")]
        public string Name { get; set; }
    }
}
