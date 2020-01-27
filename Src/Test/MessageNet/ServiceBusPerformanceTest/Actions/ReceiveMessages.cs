// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.MessageNet.Client;
using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Standard;
using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceBusPerformanceTest
{
    internal class ReceiveMessages : IAction
    {
        private readonly IOption _option;
        private readonly IMessageClient _client;
        private readonly MetricSampler _sampler = new MetricSampler(TimeSpan.FromSeconds(1));
        private int _messageCount;
        private static readonly StringVector _tag = new StringVector(nameof(ReceiveMessages));

        public ReceiveMessages(IOption option, IMessageClient client)
        {
            _option = option;
            _client = client;
        }

        public Task Run(IWorkContext context)
        {
            context.Verify(nameof(context)).IsNotNull();
            context = context
                .WithCreateLogger(nameof(ReceiveMessages))
                .With(_tag);

            context.Telemetry.Info(context, "Receiving events...");
            _messageCount = 0;

            try
            {
                _sampler.Start();
                using Timer timer = new Timer(x => MetricsOutput(context), null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));

                Task.WaitAll(new MessageReceiverHost().Run(context, _option.TaskCount, async x => await ReceiveMessage(context, x)));
            }
            catch (Exception ex)
            {
                context.Telemetry.Error(context, "Send failed", ex);
                throw;
            }
            finally
            {
                _sampler.Stop();
                MetricsOutput(context);
            }

            MetricsOutput(context);
            context.Telemetry.Info(context, $"Received {_messageCount} messages");
            return Task.CompletedTask;
        }

        private Task ReceiveMessage(IWorkContext context, NetMessage message)
        {
            _sampler.Add(1);
            return Task.FromResult(0);
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
