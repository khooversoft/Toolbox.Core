using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Standard;
using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Khooversoft.MessageNet.Client
{
    /// <summary>
    /// Primary client interface to the Message Net.
    /// </summary>
    public class MessageNetClient : IMessageNetClient
    {
        private readonly NodeRoute _nodeRoute;
        private readonly NameServerClient _nameServerClient;
        private readonly Func<string, string> _getConnectionString;
        private MessageQueueReceiveProcessor? _messageProcessor;

        public MessageNetClient(Uri nameServerUri, Func<string, string> getConnectionString)
        {
            nameServerUri.Verify(nameof(nameServerUri)).IsNotNull();
            getConnectionString.Verify(nameof(getConnectionString)).IsNotNull();

            var httpClient = new HttpClient()
            {
                BaseAddress = new Uri(nameServerUri.ToString()),
            };

            _nameServerClient = new NameServerClient(httpClient);
            _nodeRoute = new NodeRoute(_nameServerClient);
            _getConnectionString = getConnectionString;
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

            string connectionString = _getConnectionString(nodeId);
            string queueName = new Uri(nodeRegistration!.InputQueueUri).QueueName();

            return new MessageQueueSendClient(connectionString, queueName);
        }

        /// <summary>
        /// Register a receiver to receive messages
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="receiver">function to call</param>
        /// <returns>task</returns>
        public async Task RegisterReceiver(IWorkContext context, string nodeId, Func<NetMessage, Task> receiver)
        {
            nodeId.Verify(nameof(nodeId)).IsNotEmpty();
            _messageProcessor.Verify().Assert(x => x == null, "Message process has already been started");

            var request = new RouteRegistrationRequest { NodeId = nodeId };

            RouteRegistrationResponse response = await _nameServerClient.Register(context, request);
            response.Verify().IsNotNull($"Registration failed with name server");
            response.InputQueueUri!.Verify().IsNotEmpty("Name server's response did not include input queue uri");

            string connectionString = _getConnectionString(response.InputQueueUri!);
            string queueName = new Uri(response.InputQueueUri!).QueueName();

            _messageProcessor = new MessageQueueReceiveProcessor(connectionString, queueName);
            await _messageProcessor.Start(context, receiver);
        }

        public void Dispose()
        {
            var subject = Interlocked.Exchange(ref _messageProcessor, null!);
            subject?.Dispose();
        }
    }
}
