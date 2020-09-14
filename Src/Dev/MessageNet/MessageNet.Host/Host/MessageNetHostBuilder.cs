using Khooversoft.Toolbox.Standard;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Khooversoft.MessageNet.Host
{
    public class MessageNetHostBuilder
    {
        public MessageNetHostBuilder() { }

        public IMessageNetConfig? MessageNetConfig { get; set; }

        public IMessageRepository? MessageRepository { get; set; }

        public IMessageAwaiterManager? MessageAwaiterManager { get; set; }

        public IList<INodeHostReceiver> NodeReceivers { get; set; } = new List<INodeHostReceiver>();

        public MessageNetHostBuilder SetConfig(IMessageNetConfig messageNetConfig)
        {
            MessageNetConfig = messageNetConfig;
            return this;
        }

        public MessageNetHostBuilder SetRepository(IMessageRepository messageRepository)
        {
            MessageRepository = messageRepository;
            return this;
        }

        public MessageNetHostBuilder SetAwaiter(IMessageAwaiterManager messageAwaiterManager)
        {
            MessageAwaiterManager = messageAwaiterManager;
            return this;
        }

        public MessageNetHostBuilder SetNodeReceiver(params INodeHostReceiver[] nodeReceiver)
        {
            NodeReceivers = nodeReceiver.ToList();
            return this;
        }

        public MessageNetHostBuilder AddNodeReceiver(INodeHostReceiver nodeReceiver)
        {
            nodeReceiver.VerifyNotNull(nameof(nodeReceiver));

            NodeReceivers ??= (IList<INodeHostReceiver>)new List<NodeHostReceiver>();
            NodeReceivers.Add(nodeReceiver);

            return this;
        }

        public IMessageNetHost Build(ILoggerFactory loggerFactory) => new MessageNetHost(MessageNetConfig!, MessageRepository!, MessageAwaiterManager!, NodeReceivers, loggerFactory);
    }
}
