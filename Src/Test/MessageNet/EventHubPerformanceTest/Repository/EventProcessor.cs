// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EventHubPerformanceTest
{
    internal class EventProcessor : IEventProcessor
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
            }

            return partitionContext.CheckpointAsync();
        }
    }
}
