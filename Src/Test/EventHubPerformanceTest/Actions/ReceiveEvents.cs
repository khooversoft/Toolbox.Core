// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EventHubPerformanceTest
{
    public class ReceiveEvents : IAction, IEventProcessorFactory
    {
        private readonly IOption _option;
        private readonly MetricSampler _sampler = new MetricSampler(TimeSpan.FromSeconds(1));

        public ReceiveEvents(IOption option)
        {
            _option = option;
        }


        public async Task Run(IWorkContext context)
        {
            var eventProcessorHost = new EventProcessorHost(
                eventHubPath: _option.EventHub!.Name,
                consumerGroupName: PartitionReceiver.DefaultConsumerGroupName,
                eventHubConnectionString: _option.EventHub.ConnectionString,
                storageConnectionString: _option.StorageAccount!.ConnectionString,
                leaseContainerName: _option.StorageAccount.ContainerName);

            try
            {
                _sampler.Start();
                using Timer timer = new Timer(x => MetricsOutput(context), null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));

                // Registers the Event Processor Host and starts receiving messages
                await eventProcessorHost.RegisterEventProcessorFactoryAsync(this);

                while (!context.CancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(500);
                }
            }
            finally
            {
                // Disposes of the Event Processor Host
                await eventProcessorHost.UnregisterEventProcessorAsync();
                _sampler.Stop();

                MetricsOutput(context);
            }
        }

        private void MetricsOutput(IWorkContext context)
        {
            IReadOnlyList<MetricSample> samples = _sampler.GetMetrics(true);

            if( samples.Count == 0)
            {
                Console.WriteLine("Empty metrics");
                return;
            }

            foreach (var item in samples)
            {
                Console.WriteLine(item.ToString());
            }

            Console.WriteLine();
        }

        public IEventProcessor CreateEventProcessor(PartitionContext context)
        {
            return new EventProcessor(_sampler);
        }

        private class EventProcessor : IEventProcessor
        {
            private readonly MetricSampler _sampler;

            public EventProcessor(MetricSampler sampler)
            {
                _sampler = sampler;
            }

            public Task CloseAsync(PartitionContext context, CloseReason reason)
            {
                Console.WriteLine($"Processor Shutting Down. Partition '{context.PartitionId}', Reason: '{reason}'.");
                return Task.CompletedTask;
            }

            public Task OpenAsync(PartitionContext context)
            {
                Console.WriteLine($"SimpleEventProcessor initialized. Partition: '{context.PartitionId}'");
                return Task.CompletedTask;
            }

            public Task ProcessErrorAsync(PartitionContext context, Exception error)
            {
                Console.WriteLine($"Error on Partition: {context.PartitionId}, Error: {error.Message}");
                return Task.CompletedTask;
            }

            public Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
            {
                messages.Verify().IsNotNull();
                if (messages == null) return context.CheckpointAsync();

                foreach (var eventData in messages)
                {
                    var data = Encoding.UTF8.GetString(eventData.Body.Array!, eventData.Body.Offset, eventData.Body.Count);
                    _sampler.Add(1);
                    //Console.WriteLine($"Message received. Partition: '{context.PartitionId}', Data: '{data}'");
                }

                return context.CheckpointAsync();
            }
        }
    }
}
