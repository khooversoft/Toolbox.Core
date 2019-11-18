using Khooversoft.Toolbox.Standard;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceBusPerformanceTest
{
    internal class MessageClient : IMessageClient, IDisposable
    {
        private readonly IOption _option;
        private MessageSender _messageSender;
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

        public MessageClient(IOption option)
        {
            _option = option;

            _messageSender = new MessageSender(_option.ServiceBusConnectionString, _option.QueueName);
        }

        public async Task Send(IWorkContext context, string message)
        {
            await _lock.WaitAsync();

            try
            {
                if (_messageSender == null || _messageSender?.IsClosedOrClosing == true) return;

                var messageToSend = new Message(Encoding.UTF8.GetBytes(message));

                // Write the body of the message to the console
                context.Telemetry.Verbose(context, $"Sending message: {message}");

                // Send the message to the queue
                await _messageSender!.SendAsync(messageToSend);
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task Close()
        {
            await _lock.WaitAsync();

            try
            {
                MessageSender messsageSender = Interlocked.Exchange(ref _messageSender, null!);
                if (messsageSender != null)
                {
                    await messsageSender.CloseAsync();
                }
            }
            finally
            {
                _lock.Release();
            }
        }

        public void Dispose()
        {
            Close().GetAwaiter().GetResult();
        }
    }
}
