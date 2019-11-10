// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;
using Khooversoft.Toolbox.Extensions.Configuration;
using Khooversoft.Toolbox.Standard;

namespace EventHubPerformanceTest
{
    internal class EventHub
    {
        [Option("Event hub connection string")]
        public string? ConnectionString { get; set; }

        [Option("Event hub name")]
        public string? Name { get; set; }

        [Option("Consumer group name to use")]
        public string? ConsumerGroupName { get; set; }
    }
}
