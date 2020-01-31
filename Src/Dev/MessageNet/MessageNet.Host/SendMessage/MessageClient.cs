using Khooversoft.MessageNet.Client;
using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Standard;
using Microsoft.Azure.ServiceBus.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MessageNet.Host
{
    internal class MessageClient : IMessageClient
    {
        private MessageSender _messageSender;
        private readonly string _connectionString;
        private readonly string _queueName;
        private readonly AwaiterManager _awaiterManager;

        public MessageClient(string connectionString, string queueName, AwaiterManager awaiterManager)
        {
            connectionString.Verify(nameof(connectionString)).IsNotEmpty();
            queueName.Verify(nameof(queueName)).IsNotEmpty();
            awaiterManager.Verify(nameof(awaiterManager)).IsNotNull();

            _connectionString = connectionString;
            _queueName = queueName;
            _awaiterManager = awaiterManager;
            _messageSender = new MessageSender(_connectionString, _queueName);
        }

        public async Task Send(IWorkContext context, NetMessage message)
        {
            if (_messageSender == null || _messageSender?.IsClosedOrClosing == true) return;

            // Write the body of the message to the console
            context.Telemetry.Verbose(context, $"Sending message: {message}");

            // Send the message to the queue
            await _messageSender!.SendAsync(message.ConvertTo());
        }

        public Task RegisterForMessageCallBack(Guid messageId)
        {
            var tcs = new TaskCompletionSource<bool>();
            _awaiterManager.Add(messageId, tcs);

            return tcs.Task;
        }

        public async Task Close()
        {
            MessageSender messsageSender = Interlocked.Exchange(ref _messageSender, null!);
            if (messsageSender != null)
            {
                await messsageSender.CloseAsync();
            }
        }

        public void Dispose()
        {
            Close().GetAwaiter().GetResult();
        }
    }
}
