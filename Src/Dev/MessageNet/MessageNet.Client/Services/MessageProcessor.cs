using Khooversoft.Toolbox.Standard;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Khooversoft.MessageNet.Client
{
    public class MessageProcessor : IMessageProcessor, IDisposable
    {
        private Func<Message, Task>? _receiver;
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
        private MessageReceiver _messageReceiver;

        public MessageProcessor(string connectionString, string queueName)
        {
            connectionString.Verify(nameof(connectionString)).IsNotEmpty();
            queueName.Verify(nameof(queueName)).IsNotEmpty();

            _messageReceiver = new MessageReceiver(connectionString, queueName, ReceiveMode.PeekLock);
        }

        public Task Register(IWorkContext context, Func<Message, Task> receiver)
        {
            _messageReceiver.Verify().IsNotNull("MessageProcessor is closed");
            context.Verify(nameof(context)).IsNotNull();
            receiver.Verify(nameof(receiver)).IsNotNull();

            _receiver = receiver;

            var doneReceiving = new TaskCompletionSource<bool>();

            context.CancellationToken.Register(
                 async () =>
                 {
                     await Close();
                     doneReceiving.SetResult(true);
                 });

            // Configure the MessageHandler Options in terms of exception handling, number of concurrent messages to deliver etc.
            var messageHandlerOptions = new MessageHandlerOptions(x => ExceptionReceivedHandler(context, x))
            {
                // Maximum number of Concurrent calls to the callback `ProcessMessagesAsync`, set to 1 for simplicity.
                // Set it according to how many messages the application wants to process in parallel.
                MaxConcurrentCalls = 1,

                // Indicates whether MessagePump should automatically complete the messages after returning from User Callback.
                // False below indicates the Complete will be handled by the User Callback as in `ProcessMessagesAsync` below.
                AutoComplete = false
            };

            _messageReceiver.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
            return doneReceiving.Task;
        }

        public async Task Close()
        {
            await _lock.WaitAsync();

            try
            {
                MessageReceiver? receiver = Interlocked.Exchange(ref _messageReceiver, null!);
                if (receiver != null)
                {
                    await receiver.CloseAsync();
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

        private async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            await _lock.WaitAsync();

            try
            {
                if (_messageReceiver == null || message == null || _messageReceiver?.IsClosedOrClosing == true || token.IsCancellationRequested) return;

                // Process the message
                await _receiver!(message);
                //Console.WriteLine($"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{Encoding.UTF8.GetString(message.Body)}");

                // Complete the message so that it is not received again.
                // This can be done only if the queueClient is created in ReceiveMode.PeekLock mode (which is default).
                await _messageReceiver!.CompleteAsync(message.SystemProperties.LockToken);

                // Note: Use the cancellationToken passed as necessary to determine if the queueClient has already been closed.
                // If queueClient has already been Closed, you may chose to not call CompleteAsync() or AbandonAsync() etc. calls 
                // to avoid unnecessary exceptions.
            }
            finally
            {
                _lock.Release();
            }
        }

        private Task ExceptionReceivedHandler(IWorkContext context, ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            context.Telemetry.Error(context, "Message handler encountered an exception", exceptionReceivedEventArgs.Exception);

            var receiverContext = exceptionReceivedEventArgs.ExceptionReceivedContext;

            context.Telemetry.Error(context, "Exception context for troubleshooting:");
            context.Telemetry.Error(context, $"- Endpoint: {receiverContext.Endpoint}");
            context.Telemetry.Error(context, $"- Entity Path: {receiverContext.EntityPath}");
            context.Telemetry.Error(context, $"- Executing Action: {receiverContext.Action}");

            return Task.CompletedTask;
        }
    }
}
