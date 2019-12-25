using Khooversoft.MessageHub.Interface;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MessageHub.Client
{
    public class NodeRoute
    {
        private readonly ConcurrentDictionary<string, CacheObject<NodeRegistrationModel>> _routes = new ConcurrentDictionary<string, CacheObject<NodeRegistrationModel>>(StringComparer.OrdinalIgnoreCase);
        private readonly INameServerClient _nameServer;
        private readonly TimeSpan _timeToLive;

        public NodeRoute(INameServerClient nameServer, TimeSpan? timeToLive = null)
        {
            _nameServer = nameServer;
            _timeToLive = timeToLive ?? TimeSpan.FromHours(1);
        }

        public async Task<NodeRegistrationModel?> Get(IWorkContext context, string nodeId)
        {
            if (_routes.TryGetValue(nodeId, out CacheObject<NodeRegistrationModel> cacheModel))
            {
                if (cacheModel.TryGetValue(out NodeRegistrationModel model)) return model;
            }

            RouteLookupResponse? lookup = await _nameServer.Lookup(context, new RouteLookupRequest { SearchNodeId = nodeId });
            if (lookup == null) return null;

            NodeRegistrationModel lookupModel = lookup.ConvertTo();
            _routes[nodeId] = new CacheObject<NodeRegistrationModel>(_timeToLive).Set(lookupModel);

            return lookupModel;
        }
    }
}
