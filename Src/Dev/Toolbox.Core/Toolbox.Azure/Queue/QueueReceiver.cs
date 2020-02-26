using Khooversoft.Toolbox.Standard;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Azure
{
    public class QueueReceiver<T> where T : class
    {
        private Func<T, Task>? _receiver;
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
        private MessageReceiver _messageReceiver;
        private CancellationTokenSource? _cancellationTokenSource;

        public QueueReceiver(string connectionString, string queueName)
        {
            connectionString.Verify(nameof(connectionString)).IsNotEmpty();
            queueName.Verify(nameof(queueName)).IsNotEmpty();

            _messageReceiver = new MessageReceiver(connectionString, queueName, ReceiveMode.PeekLock);
        }

        public Task Start(IWorkContext context, Func<T, Task> receiver)
        {
            _messageReceiver.Verify().IsNotNull("MessageProcessor is not running");
            context.Verify(nameof(context)).IsNotNull();
            receiver.Verify(nameof(receiver)).IsNotNull();

            _receiver = receiver;

            var doneReceiving = new TaskCompletionSource<bool>();

            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(context.CancellationToken);

            _cancellationTokenSource.Token.Register(
                 () =>
                 {
                     InternalStop().GetAwaiter().GetResult();
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

        /// <summary>
        /// Stop receiver
        /// </summary>
        /// <returns></returns>
        public Task Stop()
        {
            _cancellationTokenSource.Verify().IsNotNull("Receiver is not running");

            CancellationTokenSource? cancellationTokenSource = Interlocked.Exchange(ref _cancellationTokenSource, null);
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
            }

            return Task.CompletedTask;
        }

        private async Task InternalStop()
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
            Stop().GetAwaiter().GetResult();
        }

        private async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            await _lock.WaitAsync();

            try
            {
                if (_messageReceiver == null || message == null || _messageReceiver?.IsClosedOrClosing == true || token.IsCancellationRequested) return;

                // Process the message
                string json = Encoding.UTF8.GetString(message.Body);
                T value = JsonConvert.DeserializeObject<T>(json);
                await _receiver!(value);

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
