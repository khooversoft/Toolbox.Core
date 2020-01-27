// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.MessageNet.Client;
using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Standard;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBusPerformanceTest
{
    internal class MessageReceiverHost : IDisposable
    {
        private readonly ConcurrentBag<IMessageProcessor> _messageProcessors = new ConcurrentBag<IMessageProcessor>();

        public MessageReceiverHost()
        {
        }

        /// <summary>
        /// Run receiver
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="taskCount">number of tasks</param>
        /// <param name="receiver">lambda for receiver</param>
        /// <returns></returns>
        public Task Run(IWorkContext context, int taskCount, Func<NetMessage, Task> receiver)
        {
            context.Verify(nameof(context)).IsNotNull();
            context.Container.Verify(nameof(context.Container)).IsNotNull();
            taskCount.Verify(nameof(taskCount)).Assert(x => x >= 1, "Number of task must greater or equal to 1");
            receiver.Verify(nameof(receiver)).IsNotNull();

            var tasks = Enumerable.Range(0, taskCount)
                .Select(x =>
                {
                    IMessageProcessor messageProcessor = context.Container!.Resolve<IMessageProcessor>();
                    _messageProcessors.Add(messageProcessor);
                    context.Telemetry.Info(context, "Starting receiver");
                    return messageProcessor.Start(context, receiver);
                })
                .ToList();

            return Task.WhenAll(tasks);
        }

        public async Task Close()
        {
            while(_messageProcessors.TryTake(out IMessageProcessor? messageProcessor))
            {
                await messageProcessor.Stop();
            }
        }

        public void Dispose()
        {
            Close().GetAwaiter().GetResult();
        }
    }
}
