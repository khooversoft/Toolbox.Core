// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
    public class SendEvents : IAction
    {
        private readonly IOption _option;
        private int _messageCount;

        public SendEvents(IOption option)
        {
            _option = option;
        }

        public async Task Run(IWorkContext context)
        {
            Console.WriteLine($"Sending events, Task Count:{_option.TaskCount}, ...");

            var connectionStringBuilder = new EventHubsConnectionStringBuilder(_option.EventHub!.ConnectionString);

            EventHubClient client = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());

            await SendMessagesToEventHub(context, client);

            await client.CloseAsync();

            Console.WriteLine("Completed sending events...");
        }

        private Task SendMessagesToEventHub(IWorkContext context, EventHubClient client)
        {
            var metrics = new MetricSampler(TimeSpan.FromSeconds(1))
                .Start();

            using var timer = new Timer(x => MetricsOutput(context, metrics), metrics, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));

            _messageCount = 0;

            var tasks = new List<Task>();
            Enumerable.Range(0, _option.TaskCount)
                .ForEach(x => tasks.Add(SendMessages(context, client, metrics)));

            Task.WaitAll(tasks.ToArray());
            Console.WriteLine($"Sent {_messageCount} messages");

            return Task.FromResult(0);
        }

        private async Task SendMessages(IWorkContext context, EventHubClient client, MetricSampler metrics)
        {
            for (var i = 0; (_option.Count == 0 || i < _option.Count) && !context.CancellationToken.IsCancellationRequested; i++)
            {
                try
                {
                    var message = $"Message {i} ***";
                    await client.SendAsync(new EventData(Encoding.UTF8.GetBytes(message)));
                    metrics.Add(1);
                    _messageCount++;
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"{DateTime.Now} > Exception: {exception.Message}");
                }
            }
        }

        private void MetricsOutput(IWorkContext context, MetricSampler metrics)
        {
            IReadOnlyList<MetricSample> samples = metrics.GetMetrics(true);

            if (samples.Count == 0)
            {
                Console.WriteLine("Send - empty metrics");
                return;
            }

            double total = samples.Sum(x => x.Count);
            TimeSpan span = TimeSpan.FromSeconds(samples.Sum(x => x.Span.TotalSeconds));

            Console.WriteLine($"Send: Total: {total}, Span: {span}, TPS:{total / span.TotalSeconds}");
        }
    }
}
