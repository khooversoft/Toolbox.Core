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
    internal class ReceiveEvents : IAction
    {
        private readonly IOption _option;
        private readonly IEventReceiverHost _eventReceiverHost;
        private readonly MetricSampler _sampler = new MetricSampler(TimeSpan.FromSeconds(1));
        private static int _messageCount;
        private static readonly StringVector _tag = new StringVector(nameof(ReceiveEvents));


        public ReceiveEvents(IOption option, IEventReceiverHost eventReceiverHost)
        {
            _option = option;
            _eventReceiverHost = eventReceiverHost;
        }


        public async Task Run(IWorkContext context)
        {
            context = context.With(_tag);

            context.Telemetry.Info(context, "Receiving events...");
            _messageCount = 0;

            try
            {
                _sampler.Start();
                using Timer timer = new Timer(x => MetricsOutput(context), null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));

                // Registers the Event Processor Host and starts receiving messages
                EventProcessorOptions eventOption = EventProcessorOptions.DefaultOptions;
                eventOption.ReceiveTimeout = TimeSpan.FromSeconds(5);

                await _eventReceiverHost.RegisterEventProcessorFactoryAsync(new EventProcessFactory(context, _sampler), eventOption);

                while (!context.CancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(500);
                }
            }
            catch (Exception ex)
            {
                context.Telemetry.Error(context, "Send failed", ex);
                throw;
            }
            finally
            {
                context.Telemetry.Info(context, "Unregister event processing...");

                // Disposes of the Event Processor Host
                await _eventReceiverHost.UnregisterEventProcessorAsync();
                _sampler.Stop();

                MetricsOutput(context);
            }

            MetricsOutput(context);
            context.Telemetry.Info(context, $"Received {_messageCount} messages");
        }

        private void MetricsOutput(IWorkContext context)
        {
            context = context.WithMethodName();
            IReadOnlyList<MetricSample> samples = _sampler.GetMetrics(true);

            if (samples.Count == 0)
            {
                context.Telemetry.Info(context, "Receive - empty metrics");
                return;
            }

            double total = samples.Sum(x => x.Count);
            TimeSpan span = TimeSpan.FromSeconds(samples.Sum(x => x.Span.TotalSeconds));

            context.Telemetry.Info(context, $"Receive: Total: {total}, Span: {span}, TPS:{total / span.TotalSeconds}");
        }

        private class EventProcessFactory : IEventProcessorFactory
        {
            private readonly IWorkContext _context;
            private readonly MetricSampler _sampler;

            public EventProcessFactory(IWorkContext context, MetricSampler sampler)
            {
                _context = context;
                _sampler = sampler;
            }

            public IEventProcessor CreateEventProcessor(PartitionContext context)
            {
                return new EventProcessor(_context, _sampler);
            }
        }

        private class EventProcessor : IEventProcessor
        {
            private readonly IWorkContext _context;
            private readonly MetricSampler _sampler;
            private static readonly StringVector _tag = new StringVector(nameof(EventProcessor));

            public EventProcessor(IWorkContext context, MetricSampler sampler)
            {
                _context = context.With(_tag);
                _sampler = sampler;
            }

            public Task CloseAsync(PartitionContext partitionContext, CloseReason reason)
            {
                _context.Telemetry.Verbose(_context, $"Processor Shutting Down. Partition '{partitionContext.PartitionId}', Reason: '{reason}'.");
                return Task.CompletedTask;
            }

            public Task OpenAsync(PartitionContext partitionContext)
            {
                _context.Telemetry.Verbose(_context, $"SimpleEventProcessor initialized. Partition: '{partitionContext.PartitionId}'");
                return Task.CompletedTask;
            }

            public Task ProcessErrorAsync(PartitionContext partitionContext, Exception error)
            {
                _context.Telemetry.Error(_context, $"Error on Partition: {partitionContext.PartitionId}, Error: {error.Message}", error);
                return Task.CompletedTask;
            }

            public Task ProcessEventsAsync(PartitionContext partitionContext, IEnumerable<EventData> messages)
            {
                messages.Verify().IsNotNull();
                if (messages == null) return partitionContext.CheckpointAsync();

                foreach (var eventData in messages)
                {
                    var data = Encoding.UTF8.GetString(eventData.Body.Array!, eventData.Body.Offset, eventData.Body.Count);
                    _sampler.Add(1);
                    _messageCount++;
                    _context.Telemetry.Verbose(_context, $"Message received. Partition: '{partitionContext.PartitionId}', Data: '{data}'");
                }

                return partitionContext.CheckpointAsync();
            }
        }
    }
}
