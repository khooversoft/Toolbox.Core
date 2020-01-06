using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Standard;
using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Khooversoft.MessageNet.Client
{
    /// <summary>
    /// Primary client interface to the Message Net.
    /// </summary>
    public class MessageNetClient : IMessageNetClient
    {
        private readonly string _nodeId;
        private readonly NodeRoute _nodeRoute;
        private readonly EndpointRegistration _endpointRegistration;
        private readonly NameServerClient _nameServerClient;
        private MessageProcessor? _messageProcessor;

        public MessageNetClient(string nodeId, Uri nameServerUri, params ResourceEndpointRegistration[] resourceEndpointRegistrations)
        {
            nodeId.Verify(nameof(nodeId)).IsNotEmpty();
            nameServerUri.Verify(nameof(nameServerUri)).IsNotNull();

            var httpClient = new HttpClient()
            {
                BaseAddress = new Uri(nameServerUri.ToString()),
            };

            _nodeId = nodeId;
            _nameServerClient = new NameServerClient(httpClient);
            _nodeRoute = new NodeRoute(_nameServerClient);
            _endpointRegistration = new EndpointRegistration().Add(resourceEndpointRegistrations);
        }

        /// <summary>
        /// Get a message client for a specific node id, uses name server to get route
        /// 
        /// Note: endpoint registration is required for the node
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="nodeId">node id</param>
        /// <returns>message client</returns>
        public async Task<IMessageClient> GetMessageClient(IWorkContext context, string nodeId)
        {
            nodeId.Verify(nameof(nodeId)).IsNotEmpty();

            NodeRegistrationModel? nodeRegistration = await _nodeRoute.Get(context, nodeId);
            nodeRegistration.Verify().IsNotNull($"Node {nodeId} does not exist in the name server.");

            _endpointRegistration.TryGetValue(nodeRegistration!.InputUri, out ResourceEndpointRegistration resourceEndpointRegistration)
                .Verify().Assert($"No resource endpoint registration found for {nodeRegistration.InputUri}");

            string queueName = new ResourcePathBuilder(nodeRegistration.InputUri).EntityName;

            return new MessageClient(resourceEndpointRegistration.ConnectionString, queueName);
        }

        /// <summary>
        /// Register a receiver to receive messages
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="receiver">function to call</param>
        /// <returns>task</returns>
        public async Task RegisterReceiver(IWorkContext context, Func<Message, Task> receiver)
        {
            _messageProcessor.Verify().Assert(x => x == null, "Message process has already been started");

            var request = new RouteRegistrationRequest { NodeId = _nodeId };

            RouteRegistrationResponse response = await _nameServerClient.Register(context, request);
            response.Verify().IsNotNull($"Registration failed with name server");
            response.InputQueueUri!.Verify().IsNotEmpty("Name server's response did not include input queue uri");

            _endpointRegistration.TryGetValue(response.InputQueueUri!, out ResourceEndpointRegistration resourceEndpointRegistration)
                .Verify().Assert($"No resource endpoint registration found for {response.InputQueueUri}");

            var builder = new ResourcePathBuilder(response.InputQueueUri!);

            _messageProcessor = new MessageProcessor(resourceEndpointRegistration.ConnectionString, builder.EntityName);
            await _messageProcessor.Register(context, receiver);
        }
    }
}
