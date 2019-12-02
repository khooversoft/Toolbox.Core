using Khooversoft.MessageHub.Interface;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessageHub.Client
{
    public class MessageBusClient
    {
        private readonly INameServerClient _nameServer;
        private readonly NodeRoute _nodeRoute;
        private readonly EndpointRegistration _endpointRegistration;

        public MessageBusClient(INameServerClient nameServer, EndpointRegistration endpointRegistration)
        {
            nameServer.Verify(nameof(nameServer)).IsNotNull();
            endpointRegistration.Verify(nameof(endpointRegistration)).IsNotNull();

            _nameServer = nameServer;
            _nodeRoute = new NodeRoute(nameServer);
            _endpointRegistration = endpointRegistration;
        }

        public async Task<IMessageClient> GetMessageClient(IWorkContext context, string nodeId)
        {
            nodeId.Verify(nameof(nodeId)).IsNotEmpty();

            NodeRegistrationModel? nodeRegistration = await _nodeRoute.Get(context, nodeId);
            nodeRegistration.Verify().IsNotNull($"Node {nodeId} does not exist in the name server.");

            _endpointRegistration.TryGetValue(nodeRegistration.InputUri, out ResourceEndpointRegistration resourceEndpointRegistration)
                .Verify().Assert(x => x == true, $"No resource endpoint registration found for {nodeRegistration.InputUri}");

            string queueName = new ResourcePathBuilder(nodeRegistration.InputUri).EntityName;

            return new MessageClient(resourceEndpointRegistration.ConnectionString, queueName);
        }
    }
}
