// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Standard;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Khooversoft.MessageNet.Host
{
    internal class MessageClient : IMessageClient
    {
        private MessageSender _messageSender;
        private readonly string _connectionString;
        private readonly string _queueName;
        private readonly IMessageAwaiterManager _awaiterManager;
        private readonly ILogger<MessageClient> _logger;

        public MessageClient(string connectionString, string queueName, IMessageAwaiterManager awaiterManager, ILogger<MessageClient> logger)
        {
            connectionString.VerifyNotEmpty(nameof(connectionString));
            queueName.VerifyNotEmpty(nameof(queueName));
            awaiterManager.VerifyNotNull(nameof(awaiterManager));

            _connectionString = connectionString;
            _queueName = queueName;
            _awaiterManager = awaiterManager;
            _logger = logger;
            _messageSender = new MessageSender(_connectionString, _queueName);
        }

        /// <summary>
        /// Send message, fire and forget
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="message">message</param>
        /// <returns>task</returns>
        public async Task Send(NetMessage message)
        {
            if (_messageSender == null || _messageSender?.IsClosedOrClosing == true) return;
            message.Header.ToUri.ToMessageUri().ToQueueId().GetQueueName().VerifyAssert(x => x == _queueName, $"Message ToUri {message.Header.ToUri} does not match queueName {_queueName}");

            // Write the body of the message to the console
            _logger.LogTrace($"Sending message: {message}");

            // Send the message to the queue
            await _messageSender!.SendAsync(message.ToMessage());
        }

        /// <summary>
        /// Send message and wait for response
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="message">message</param>
        /// <returns>task</returns>
        public async Task<NetMessage> Call(NetMessage message, TimeSpan? timeout = null)
        {
            if (_messageSender == null || _messageSender?.IsClosedOrClosing == true) throw new InvalidOperationException("Sender has been closed or is closing");
            message.Header.ToUri.ToMessageUri().ToQueueId().GetQueueName().VerifyAssert(x => x == _queueName, $"Message ToUri {message.Header.ToUri} does not match queueName {_queueName}");

            // Write the body of the message to the console
            _logger.LogTrace($"Calling message: {message}");

            // Send the message to the queue
            await _messageSender!.SendAsync(message.ToMessage());

            // Wait for response
            return await WaitForResponse(message.Header.MessageId, timeout);
        }

        /// <summary>
        /// Wait for response from message that was sent
        /// </summary>
        /// <param name="messageId">message id to wait for</param>
        /// <returns>task</returns>
        private Task<NetMessage> WaitForResponse(Guid messageId, TimeSpan? timeout)
        {
            var tcs = new TaskCompletionSource<NetMessage>();
            _awaiterManager.Add(messageId, tcs, timeout);

            return tcs.Task;
        }

        /// <summary>
        /// Close client
        /// </summary>
        /// <returns>task</returns>
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
