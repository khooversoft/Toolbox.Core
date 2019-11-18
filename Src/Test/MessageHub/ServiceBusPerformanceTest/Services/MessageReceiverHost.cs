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
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="taskCount"></param>
        /// <param name="receiver"></param>
        /// <returns></returns>
        public Task Run(IWorkContext context, int taskCount, Func<Message, Task> receiver)
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
                    return messageProcessor.Register(context, receiver);
                })
                .ToList();

            return Task.WhenAll(tasks);
        }

        public async Task Close()
        {
            while(_messageProcessors.TryTake(out IMessageProcessor? messageProcessor))
            {
                await messageProcessor.Close();
            }
        }

        public void Dispose()
        {
            Close().GetAwaiter().GetResult();
        }
    }
}
