// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Standard;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Khooversoft.MessageNet.Client
{
    public class MessageQueueSendClient : IMessageClient, IDisposable
    {
        private MessageSender _messageSender;
        private readonly string _connectionString;
        private readonly string _queueName;

        public MessageQueueSendClient(string connectionString, string queueName)
        {
            connectionString.Verify(nameof(connectionString)).IsNotEmpty();
            queueName.Verify(nameof(queueName)).IsNotEmpty();

            _connectionString = connectionString;
            _queueName = queueName;
            _messageSender = new MessageSender(_connectionString, _queueName);
        }

        public async Task Send(IWorkContext context, string message)
        {
            if (_messageSender == null || _messageSender?.IsClosedOrClosing == true) return;

            var messageToSend = new Message(message.ToByteArray());

            // Write the body of the message to the console
            context.Telemetry.Verbose(context, $"Sending message: {message}");

            // Send the message to the queue
            await _messageSender!.SendAsync(messageToSend);
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
