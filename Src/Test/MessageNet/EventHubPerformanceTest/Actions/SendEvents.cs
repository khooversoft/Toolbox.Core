// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Autofac.Features.OwnedInstances;
using Khooversoft.Toolbox.Standard;
using Microsoft.Azure.EventHubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EventHubPerformanceTest
{
    internal class SendEvents : IAction
    {
        private readonly IOption _option;
        private readonly Owned<ISendEvent> _sendEvent;
        private int _messageCount;

        public SendEvents(IOption option, Owned<ISendEvent> eventSend)
        {
            _option = option;
            _sendEvent = eventSend;
        }

        public async Task Run(IWorkContext context)
        {
            context.VerifyNotNull(nameof(context));
            context = context
                .WithCreateLogger(nameof(SendEvents))
                .WithActivity();

            context.Telemetry.Info(context, $"Sending events, Task Count:{_option.TaskCount}, ...");

            try
            {
                await SendMessagesToEventHub(context);
            }
            finally
            {
                await _sendEvent.Value.CloseAsync(context);
                _sendEvent.Dispose();
            }

            context.Telemetry.Info(context, "Completed sending events...");
        }

        private Task SendMessagesToEventHub(IWorkContext context)
        {
            var metrics = new MetricSampler(TimeSpan.FromSeconds(1))
                .Start();

            using var timer = new Timer(x => MetricsOutput(context, metrics), metrics, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));

            _messageCount = 0;

            var tasks = new List<Task>();
            Enumerable.Range(0, _option.TaskCount)
                .ForEach(x => tasks.Add(SendMessages(context, metrics)));

            Task.WaitAll(tasks.ToArray());
            Console.WriteLine($"Sent {_messageCount} messages");

            return Task.FromResult(0);
        }

        private async Task SendMessages(IWorkContext context, MetricSampler metrics)
        {
            for (var i = 0; (_option.Count == 0 || i < _option.Count) && !context.CancellationToken.IsCancellationRequested; i++)
            {
                try
                {
                    var message = $"Message {i} ***";
                    await _sendEvent.Value.SendAsync(context, new EventData(Encoding.UTF8.GetBytes(message)));
                    metrics.Add(1);
                    _messageCount++;
                }
                catch (Exception exception)
                {
                    context.Telemetry.Error(context, $"{DateTime.Now} > Exception: {exception.Message}");
                }
            }
        }

        private void MetricsOutput(IWorkContext context, MetricSampler metrics)
        {
            IReadOnlyList<MetricSample> samples = metrics.GetMetrics(true);

            if (samples.Count == 0)
            {
                context.Telemetry.Info(context, "Send - empty metrics");
                return;
            }

            double total = samples.Sum(x => x.Count);
            TimeSpan span = TimeSpan.FromSeconds(samples.Sum(x => x.Span.TotalSeconds));

            context.Telemetry.Info(context, $"Send: Total: {total}, Span: {span}, TPS:{total / span.TotalSeconds}");
        }
    }
}
