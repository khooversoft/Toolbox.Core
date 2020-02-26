using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Khooversoft.MessageNet.Interface
{
    public class QueueId
    {
        private static readonly Regex _idVerify = new Regex(@"^[a-zA-Z][a-zA-Z0-9]+$", RegexOptions.Compiled);

        private static readonly Regex _nodeIdVerify = new Regex(@"^[a-zA-Z][a-zA-Z0-9\.]+$", RegexOptions.Compiled);

        public QueueId(string nameSpace, string networkId, string nodeId)
        {
            Verify(nameSpace, networkId, nodeId);

            Namespace = nameSpace;
            NetworkId = networkId;
            NodeId = nodeId;
        }

        public string Namespace { get; }

        public string NetworkId { get; }

        public string NodeId { get; }

        public override string ToString() => Namespace + "/" + NetworkId + "/" + NodeId;

        public string GetQueueName() => NetworkId + "/" + NodeId;

        public override bool Equals(object obj) => obj switch
        {
            QueueId queueId => Namespace.Equals(queueId.NetworkId, StringComparison.OrdinalIgnoreCase) &&
                NetworkId.Equals(queueId.NetworkId, StringComparison.OrdinalIgnoreCase) &&
                NodeId.Equals(queueId.NodeId, StringComparison.OrdinalIgnoreCase),

            _ => false,
        };

        public override int GetHashCode() => HashCode.Combine(NetworkId, NodeId);

        public static bool operator ==(QueueId v1, QueueId v2) => v1?.Equals(v2) == true;

        public static bool operator !=(QueueId v1, QueueId v2) => v1?.Equals(v2) == false;

        public static QueueId Parse(string queueId)
        {
            queueId.Verify(nameof(queueId)).IsNotEmpty();

            var parts = queueId.Split('/', StringSplitOptions.RemoveEmptyEntries);
            parts.Verify().Assert(x => x.Length == 3, "Invalid format for queue id");

            return new QueueId(parts[0], parts[1], parts[2]);
        }

        public static bool IsValid(string? nameSpace, string? networkId, string? nodeId)
        {
            return !networkId.IsEmpty() &&
                !nameSpace.IsEmpty() &&
                !nodeId.IsEmpty() &&
                _idVerify.IsMatch(nameSpace) &&
                _idVerify.IsMatch(networkId) &&
                _nodeIdVerify.IsMatch(nodeId);
        }

        public static bool IsValid(string? networkId, string? nodeId)
        {
            return !networkId.IsEmpty() &&
                !nodeId.IsEmpty() &&
                _idVerify.IsMatch(networkId) &&
                _nodeIdVerify.IsMatch(nodeId);
        }

        public static void Verify(string? nameSpace, string? networkId, string? nodeId)
        {
            nameSpace.Verify(nameof(nameSpace)).Assert(_idVerify.IsMatch(nameSpace), "Namespace id is not valid: [alpha][alpha, numeric, ...]");
            networkId.Verify(nameof(networkId)).Assert(_idVerify.IsMatch(networkId), "Network id is not valid: [alpha][alpha, numeric, ...]");
            nodeId.Verify(nameof(nodeId)).Assert(_nodeIdVerify.IsMatch(nodeId), "Node id is not valid: [alpha][alpha, numeric, '.', ...]");
        }
    }
}
