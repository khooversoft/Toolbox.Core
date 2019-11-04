// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EventHubPerformanceTest
{
    public class ReceiveEvents : IAction, IEventProcessorFactory
    {
        private readonly IOption _option;
        private readonly MetricSampler _sampler = new MetricSampler(TimeSpan.FromSeconds(1));
        private static int _messageCount;

        public ReceiveEvents(IOption option)
        {
            _option = option;
        }


        public async Task Run(IWorkContext context)
        {
            Console.WriteLine("Receiving events...");
            _messageCount = 0;

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
                EventProcessorOptions eventOption = EventProcessorOptions.DefaultOptions;
                eventOption.ReceiveTimeout = TimeSpan.FromSeconds(5);

                await eventProcessorHost.RegisterEventProcessorFactoryAsync(this, eventOption);

                while (!context.CancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(500);
                }
            }
            finally
            {
                Console.WriteLine("Unregister event processing...");

                // Disposes of the Event Processor Host
                await eventProcessorHost.UnregisterEventProcessorAsync();
                _sampler.Stop();

                MetricsOutput(context);
            }

            MetricsOutput(context);
            Console.WriteLine($"Received {_messageCount} messages");
        }

        private void MetricsOutput(IWorkContext context)
        {
            IReadOnlyList<MetricSample> samples = _sampler.GetMetrics(true);

            if (samples.Count == 0)
            {
                Console.WriteLine("Receive - empty metrics");
                return;
            }

            double total = samples.Sum(x => x.Count);
            TimeSpan span = TimeSpan.FromSeconds(samples.Sum(x => x.Span.TotalSeconds));

            Console.WriteLine($"Receive: Total: {total}, Span: {span}, TPS:{total / span.TotalSeconds}");
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
                    _messageCount++;
                    //Console.WriteLine($"Message received. Partition: '{context.PartitionId}', Data: '{data}'");
                }

                return context.CheckpointAsync();
            }
        }
    }
}
