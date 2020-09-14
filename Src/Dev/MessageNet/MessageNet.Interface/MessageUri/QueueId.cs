using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Khooversoft.MessageNet.Interface
{
    public class QueueId
    {
        private static readonly Regex _idVerify = new Regex(@"^[a-zA-Z][a-zA-Z0-9]*$", RegexOptions.Compiled);
        private static readonly Regex _nodeIdVerify = new Regex(@"^[a-zA-Z][a-zA-Z0-9\.]*$", RegexOptions.Compiled);

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

        public string GetQueueName() => NetworkId + "/" + NodeId;

        public override string ToString() => Namespace + "/" + NetworkId + "/" + NodeId;

        public static QueueId Parse(string queueId)
        {
            queueId.VerifyNotEmpty(nameof(queueId));

            var parts = queueId.Split('/', StringSplitOptions.RemoveEmptyEntries);
            parts.VerifyAssert(x => x.Length == 3, "Invalid format for queue id");

            return new QueueId(parts[0], parts[1], parts[2]);
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
            nameSpace.VerifyAssert(x => _idVerify.IsMatch(x), "Namespace id is not valid: [alpha][alpha, numeric, ...]");
            networkId.VerifyAssert(x => _idVerify.IsMatch(networkId), "Network id is not valid: [alpha][alpha, numeric, ...]");
            nodeId.VerifyAssert(x => _nodeIdVerify.IsMatch(nodeId), "Node id is not valid: [alpha][alpha, numeric, '.', ...]");
        }

        public override bool Equals(object? obj)
        {
            return obj is QueueId id &&
                   Namespace.Equals(id.Namespace, StringComparison.OrdinalIgnoreCase) &&
                   NetworkId.Equals(id.NetworkId, StringComparison.OrdinalIgnoreCase) &&
                   NodeId.Equals(id.NodeId, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode() => HashCode.Combine(Namespace, NetworkId, NodeId);

        public static bool operator ==(QueueId? left, QueueId? right) => EqualityComparer<QueueId>.Default.Equals(left!, right!);

        public static bool operator !=(QueueId? left, QueueId? right) => !(left == right);
    }
}
