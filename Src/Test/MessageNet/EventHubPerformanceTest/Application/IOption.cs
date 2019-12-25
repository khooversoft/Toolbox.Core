// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;

namespace EventHubPerformanceTest
{
    internal interface IOption
    {
        bool Help { get; }

        bool Send { get; }

        bool Receive { get; }

        int Count { get; }

        int TaskCount { get; }

        string LoggingFolder { get; }

        TelemetryType ConsoleLevel { get; }

        EventHub? EventHub { get; }

        StorageAccount? StorageAccount { get; }
    }
}