// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using Microsoft.Azure.EventHubs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EventHubPerformanceTest
{
    public class SendEvents : IAction
    {
        private readonly IOption _option;

        public SendEvents(IOption option)
        {
            _option = option;
        }

        public async Task Run(IWorkContext context)
        {
            var connectionStringBuilder = new EventHubsConnectionStringBuilder(_option.EventHub!.ConnectionString);

            EventHubClient client = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());

            await SendMessagesToEventHub(context, client);

            await client.CloseAsync();
        }

        private async Task SendMessagesToEventHub(IWorkContext context, EventHubClient client)
        {
            var metrics = new MetricSampler(TimeSpan.FromSeconds(1))
                .Start();

            using var timer = new Timer(x => MetricsOutput(context, metrics), metrics, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));

            for (var i = 0; (_option.Count == 0 || i < _option.Count) && !context.CancellationToken.IsCancellationRequested; i++)
            {
                try
                {
                    var message = $"Message {i} ***";
                    //Console.WriteLine($"Sending message: {message}");
                    await client.SendAsync(new EventData(Encoding.UTF8.GetBytes(message)));
                    metrics.Add(1);
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
                Console.WriteLine("Empty metrics");
                return;
            }

            foreach (var item in samples)
            {
                Console.WriteLine(item.ToString());
            }

            Console.WriteLine();
        }
    }
}
