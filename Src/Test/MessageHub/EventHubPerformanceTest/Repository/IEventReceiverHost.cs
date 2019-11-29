// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.Azure.EventHubs.Processor;

namespace EventHubPerformanceTest
{
    internal interface IEventReceiverHost
    {
        Task RegisterEventProcessorFactoryAsync(IEventProcessorFactory factory, EventProcessorOptions processorOptions);

        Task UnregisterEventProcessorAsync();
    }
}