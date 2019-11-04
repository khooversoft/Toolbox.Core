// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;
using Khooversoft.Toolbox.Core.Extensions.Configuration;

namespace EventHubPerformanceTest
{
    public class EventHub
    {
        [Option("Event hub connection string")]
        public string? ConnectionString { get; set; }

        [Option("Event hub name")]
        public string? Name { get; set; }

        [Option("Namespaces to use")]
        public string[]? Namespaces { get; set; }
    }
}
