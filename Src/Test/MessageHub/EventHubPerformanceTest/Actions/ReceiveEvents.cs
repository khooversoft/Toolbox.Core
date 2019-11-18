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
        private int _messageCount;
        private static readonly StringVector _tag = new StringVector(nameof(ReceiveEvents));

        public ReceiveEvents(IOption option, IEventReceiverHost eventReceiverHost)
        {
            _option = option;
            _eventReceiverHost = eventReceiverHost;
        }


        public async Task Run(IWorkContext context)
        {
            context.Verify(nameof(context)).IsNotNull();
            context = context
                .WithCreateLogger(nameof(ReceiveEvents))
                .With(_tag);

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

            int total = samples.Sum(x => x.Count);
            _messageCount += total;

            TimeSpan span = TimeSpan.FromSeconds(samples.Sum(x => x.Span.TotalSeconds));

            context.Telemetry.Info(context, $"Receive: Total: {total}, Span: {span}, TPS:{total / span.TotalSeconds}");
        }
    }
}
