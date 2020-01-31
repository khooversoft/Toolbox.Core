using Khooversoft.MessageNet.Client;
using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Actor;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MessageNet.Host
{
    internal class NodeRouteActor : ActorBase, INodeRouteActor
    {
        private readonly INameServerClient _nameServer;
        private readonly CacheObject<NodeRegistrationModel> _cache = new CacheObject<NodeRegistrationModel>(TimeSpan.FromHours(1));
        private readonly string _networkId;
        private readonly string _nodeId;

        public NodeRouteActor(INameServerClient nameServer)
        {
            _nameServer = nameServer;

            var netMessageUri = MessageUriBuilder.Parse(base.ActorKey.VectorKey).Build();

            _networkId = netMessageUri.NetworkId;
            _nodeId = netMessageUri.NodeId;
        }

        public async Task<NodeRegistrationModel?> Lookup(IWorkContext context)
        {
            if (_cache.TryGetValue(out NodeRegistrationModel cachedValue)) return cachedValue;

            RouteLookupResponse? lookup = await _nameServer.Lookup(context, new RouteLookupRequest { NetworkId = _networkId, NodeId = _nodeId });
            if (lookup == null) return null;

            NodeRegistrationModel lookupModel = lookup.ConvertTo();

            _cache.Set(lookupModel);
            return lookupModel;
        }

        public async Task<NodeRegistrationModel> Register(IWorkContext context)
        {
            _cache.Clear();

            var request = new RouteRegistrationRequest { NetworkId = _networkId, NodeId = _nodeId };

            RouteRegistrationResponse response = await _nameServer.Register(context, request);
            response.Verify().IsNotNull($"Registration failed with name server");
            response.InputQueueUri!.Verify().IsNotEmpty("Name server's response did not include input queue URI");

            NodeRegistrationModel nodeRegistrationModel = response.ConvertTo();
            _cache.Set(nodeRegistrationModel);

            return nodeRegistrationModel;
        }
    }
}
