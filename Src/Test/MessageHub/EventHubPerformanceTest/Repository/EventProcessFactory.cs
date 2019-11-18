using Khooversoft.Toolbox.Standard;
using Microsoft.Azure.EventHubs.Processor;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventHubPerformanceTest
{
    internal class EventProcessFactory : IEventProcessorFactory
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
}
