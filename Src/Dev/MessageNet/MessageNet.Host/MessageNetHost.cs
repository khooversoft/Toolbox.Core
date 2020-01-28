using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageNet.Host
{
    public class MessageNetHost : IMessageNetHost
    {
        private readonly Uri _nameServerUri;
        private readonly IConnectionManager _connectionManager;
        private readonly IEnumerable<NodeHostRegistration> _nodeRegistrations;
        private readonly IAwaiterManager _awaiterManager = new AwaiterManager();

        public MessageNetHost(Uri nameServerUri, IConnectionManager connectionManager, IEnumerable<NodeHostRegistration> nodeRegistrations)
        {
            nameServerUri.Verify(nameof(nameServerUri)).IsNotNull();
            connectionManager.Verify(nameof(connectionManager)).IsNotNull();
            nameServerUri.Verify(nameof(nameServerUri)).IsNotNull();
            nodeRegistrations.Verify(nameof(nodeRegistrations)).IsNotNull();

            _nameServerUri = nameServerUri;
            _connectionManager = connectionManager;

            _nodeRegistrations = nodeRegistrations
                .ToList()
                .Verify(nameof(nodeRegistrations)).Assert(x => x.Count > 0, "Node registrations are required")
                .Value;
        }

        public async Task Run()
        {
            IReadOnlyList<NodeHostActor> nodeHost = 
        }
    }
}
