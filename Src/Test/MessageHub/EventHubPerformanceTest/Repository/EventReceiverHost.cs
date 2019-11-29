// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using Microsoft.Azure.EventHubs.Processor;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EventHubPerformanceTest
{
    internal class EventReceiverHost : IEventReceiverHost
    {
        private readonly IOption _option;
        private readonly EventProcessorHost _host;
        private static readonly StringVector _tag = new StringVector(nameof(ReceiveEvents));

        public EventReceiverHost(IOption option)
        {
            _option = option;

            _host = new EventProcessorHost(
                eventHubPath: _option.EventHub!.Name,
                consumerGroupName: _option.EventHub!.ConsumerGroupName,
                eventHubConnectionString: _option.EventHub.ConnectionString,
                storageConnectionString: _option.StorageAccount!.ConnectionString,
                leaseContainerName: _option.StorageAccount.ContainerName);
        }

        public Task RegisterEventProcessorFactoryAsync(IEventProcessorFactory factory, EventProcessorOptions processorOptions) => _host.RegisterEventProcessorFactoryAsync(factory, processorOptions);

        public Task UnregisterEventProcessorAsync() => _host.UnregisterEventProcessorAsync();
    }
}
