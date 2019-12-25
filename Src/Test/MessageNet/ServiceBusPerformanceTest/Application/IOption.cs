// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;

namespace ServiceBusPerformanceTest
{
    internal interface IOption
    {
        TelemetryType ConsoleLevel { get; }
        int Count { get; }
        bool Help { get; set; }
        string? LoggingFolder { get; }
        string? QueueName { get; set; }
        bool Receive { get; }
        bool Send { get; }
        string? ServiceBusConnectionString { get; set; }
        int TaskCount { get; }
    }
}