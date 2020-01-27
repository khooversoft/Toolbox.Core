using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Standard;
using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Khooversoft.MessageNet.Client
{
    /// <summary>
    /// Primary client interface to the Message Net.
    /// </summary>
    public class MessageNetClientFake : IMessageNetClient
    {
        private ActionBlock<NetMessage>? _messageBlock;
        private Func<NetMessage, Task>? _receiver;

        public MessageNetClientFake()
        {
        }

        /// <summary>
        /// Get a message client for a specific node id, uses name server to get route
        /// 
        /// Note: endpoint registration is required for the node
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="nodeId">node id</param>
        /// <returns>message client</returns>
        public Task<IMessageClient> GetMessageClient(IWorkContext context, string nodeId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Register a receiver to receive messages
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="receiver">function to call</param>
        /// <returns>task</returns>
        public Task RegisterReceiver(IWorkContext context, string nodeId, Func<NetMessage, Task> receiver)
        {
            context.Verify(nameof(context)).IsNotNull();
            nodeId.Verify(nameof(nodeId)).IsNotEmpty();
            _receiver.Verify().Assert(x => x == null, "Message process has already been started");

            _receiver = receiver;
            _messageBlock = new ActionBlock<NetMessage>(x => _receiver(x));
            return Task.CompletedTask;
        }

        /// <summary>
        /// Send message to receiver
        /// </summary>
        /// <param name="messageData"></param>
        /// <returns></returns>
        public Task SendMessage(NetMessage netMessage)
        {
            netMessage.Verify(nameof(netMessage)).IsNotNull();
            _receiver.Verify().Assert(x => x != null, "Message process has not been started");

            _messageBlock!.Post(netMessage);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            var currentActionBlock = Interlocked.Exchange(ref _messageBlock, null!);
            if (currentActionBlock != null)
            {
                currentActionBlock!.Complete();
                currentActionBlock.Completion.GetAwaiter().GetResult();
            }

            _receiver = null;
        }
    }
}
