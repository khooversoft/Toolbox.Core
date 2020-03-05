using Autofac;
using FluentAssertions;
using Khooversoft.MessageNet.Host;
using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Azure;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MessageNet.Host.Tests
{
    /// <summary>
    /// The 2 node test is to route a message from one node another and back with a response
    /// 
    /// In this model, there are two nodes, client and the identity.  Client will send a message
    /// and the identity will response.
    /// </summary>
    [Collection("QueueTests")]
    public class TwoNodeTests : IClassFixture<ApplicationFixture>
    {
        private readonly IWorkContext _workContext = WorkContextBuilder.Default;
        private readonly QueueManagement _queueManagement;
        private readonly ApplicationFixture _application;

        public TwoNodeTests(ApplicationFixture application)
        {
            _queueManagement = new QueueManagement(application.ConnectionString);
            _application = application;
        }

        [Fact]
        public async Task GivenTwoNodes_UsingHost_MessagesArePassed()
        {
            var clientQueueId = new QueueId("default", "test", "clientNode");
            var identityQueueId = new QueueId("default", "test", "identityNode");

            ILifetimeScope container = _application.CreateContainer();

            IMessageRepository messageRepository = container.Resolve<IMessageRepository>();
            await messageRepository.Unregister(_workContext, clientQueueId);
            await messageRepository.Unregister(_workContext, identityQueueId);

            IMessageNetHost netHost = container.Resolve<IMessageNetHost>();

            IMessageClient toClient = await netHost.GetMessageClient(_workContext, clientQueueId);
            IMessageClient toIdentity = await netHost.GetMessageClient(_workContext, identityQueueId);
            var clientReceiverTask = new TaskCompletionSource<NetMessage>();

            Func<NetMessage, Task> clientNodeReceiver = x =>
            {
                clientReceiverTask.SetResult(x);
                return Task.CompletedTask;
            };

            Func<NetMessage, Task> identityNodeReceiver = async x =>
            {
                NetMessage netMessage = x.WithAddToTop(new MessageHeader(x.Header.FromUri, x.Header.ToUri, "post"));

                await toClient.Send(_workContext, netMessage);
            };

            var nodeHostRegistrations = new NodeHostRegistration[]
            {
                new NodeHostRegistration(identityQueueId, identityNodeReceiver),
                new NodeHostRegistration(clientQueueId, clientNodeReceiver),
            };

            await netHost.Start(_workContext, nodeHostRegistrations);

            var header = new MessageHeader("ns/test/toUri", "ns/test/fromUri", "post");

            var message = new NetMessageBuilder()
                .Add(header)
                .Build();

            await toIdentity.Send(_workContext, message);

            NetMessage receivedMessage = await clientReceiverTask.Task;

            receivedMessage.Headers.Count.Should().Be(2);
            receivedMessage.Headers.Skip(1).First().Should().Be(header);
        }
    }
}
