// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace EventHubPerformanceTest
{
    public interface IOption
    {
        bool Help { get; }

        bool Send { get; }

        bool Receive { get; }

        int Count { get; }

        EventHub? EventHub { get; }

        StorageAccount? StorageAccount { get; }
    }
}