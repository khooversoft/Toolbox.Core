using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MessageNet.Host
{
    public class NodeHostRegistration
    {
        public NodeHostRegistration(string networkId, string nodeId, Func<NetMessage, Task> receiver)
        {
            networkId.Verify(nameof(networkId)).IsNotEmpty();
            nodeId.Verify(nameof(nodeId)).IsNotEmpty();
            receiver.Verify(nameof(receiver)).IsNotNull();

            NetworkId = networkId;
            NodeId = nodeId;
            Receiver = receiver;
        }

        public string NetworkId { get; }

        public string NodeId { get; }

        public Func<NetMessage, Task> Receiver { get; }

        public string QueueName => NetworkId + "/" + NodeId;
    }
}
