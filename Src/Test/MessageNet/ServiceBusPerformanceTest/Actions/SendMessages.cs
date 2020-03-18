// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.MessageNet.Host;
using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceBusPerformanceTest
{
    internal class SendMessages : IAction
    {
        private readonly IOption _option;
        private readonly IMessageClient _client;
        private int _messageCount;

        public SendMessages(IOption option, IMessageClient client)
        {
            _option = option;
            _client = client;
        }

        public async Task Run(IWorkContext context)
        {
            context.Verify(nameof(context)).IsNotNull();
            context = context
                .WithCreateLogger(nameof(SendMessages))
                .WithActivity();

            context.Telemetry.Info(context, $"Sending messages, Task Count:{_option.TaskCount}, ...");

            await SendMessageToServiceBus(context);

            context.Telemetry.Info(context, "Completed sending messages...");
        }

        private Task SendMessageToServiceBus(IWorkContext context)
        {
            var metrics = new MetricSampler(TimeSpan.FromSeconds(1))
                .Start();

            using var timer = new Timer(x => MetricsOutput(context, metrics), metrics, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));

            _messageCount = 0;

            var tasks = new List<Task>();
            Enumerable.Range(0, _option.TaskCount)
                .ForEach(x => tasks.Add(SendMessage(context, metrics)));

            Task.WaitAll(tasks.ToArray());
            Console.WriteLine($"Sent {_messageCount} messages");

            return Task.FromResult(0);
        }

        private async Task SendMessage(IWorkContext context, MetricSampler metrics)
        {
            for (var i = 0; (_option.Count == 0 || i < _option.Count) && !context.CancellationToken.IsCancellationRequested; i++)
            {
                try
                {
                    var message = $"Message {i} ***";
                    NetMessage netMessage = new NetMessageBuilder()
                        .Add(new MessageHeader("net1/node2", "net1/node1", "post"))
                        .Add(MessageContent.Create(message))
                        .Build();

                    await _client.Send(context, netMessage);
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
