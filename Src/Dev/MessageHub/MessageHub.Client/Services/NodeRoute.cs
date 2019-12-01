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
        private readonly INameServer _nameServer;
        private readonly TimeSpan _timeToLive;

        public NodeRoute(INameServer nameServer, TimeSpan? timeToLive = null)
        {
            _nameServer = nameServer;
            _timeToLive = timeToLive ?? TimeSpan.FromHours(1);
        }

        public async Task<NodeRegistrationModel> Get(IWorkContext context, string nodeId)
        {
            if (_routes.TryGetValue(nodeId, out CacheObject<NodeRegistrationModel> cacheModel))
            {
                if (cacheModel.TryGetValue(out NodeRegistrationModel model)) return model;
            }

            NodeRegistrationModel lookup = await _nameServer.Lookup(nodeId);
            if (lookup != null) _routes[nodeId] = new CacheObject<NodeRegistrationModel>(_timeToLive).Set(lookup);

            return lookup;
        }
    }
}
