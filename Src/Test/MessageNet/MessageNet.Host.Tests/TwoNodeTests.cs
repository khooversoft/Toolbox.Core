using FluentAssertions;
using Khoover.Toolbox.TestTools;
using Khooversoft.MessageNet.Host;
using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Standard;
using MessageNet.Host.Tests.Application;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
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
        private readonly ApplicationFixture _application;
        private readonly ILoggerFactory _loggerFactory = new TestLoggerFactory();

        public TwoNodeTests(ApplicationFixture application)
        {
            _application = application;
        }

        [Fact]
        public async Task GivenTwoNodes_UsingHostSend_MessagesArePassed()
        {
            var clientQueueId = new QueueId("default", "test", "clientNode");
            var identityQueueId = new QueueId("default", "test", "identityNode");

            IMessageRepository messageRepository = new MessageRepository(_application.GetMessageNetConfig(), _loggerFactory);
            await messageRepository.Unregister(clientQueueId, CancellationToken.None);
            await messageRepository.Unregister(identityQueueId, CancellationToken.None);

            IMessageNetHost netHost = null!;

            var clientReceiverTask = new TaskCompletionSource<NetMessage>();

            Func<NetMessage, Task> clientNodeReceiver = x =>
            {
                clientReceiverTask.SetResult(x);
                return Task.CompletedTask;
            };

            Func<NetMessage, Task> identityNodeReceiver = async x =>
            {
                NetMessage netMessage = new NetMessageBuilder(x)
                    .Add(x.Header.WithReply("get.response"))
                    .Build();

                await netHost.Send(netMessage);
            };

            netHost = new MessageNetHostBuilder()
                .SetConfig(_application.GetMessageNetConfig())
                .SetRepository(new MessageRepository(_application.GetMessageNetConfig(), _loggerFactory))
                .SetAwaiter(new MessageAwaiterManager())
                .AddNodeReceiver(new NodeHostReceiver(identityQueueId, identityNodeReceiver))
                .AddNodeReceiver(new NodeHostReceiver(clientQueueId, clientNodeReceiver))
                .Build(_loggerFactory);

            await netHost.Start(CancellationToken.None);

            var header = new MessageHeader(identityQueueId.ToMessageUri(), clientQueueId.ToMessageUri(), "get");

            var message = new NetMessageBuilder()
                .Add(header)
                .Build();

            await netHost.Send(message);

            NetMessage receivedMessage = await clientReceiverTask.Task;

            receivedMessage.Headers.Count.Should().Be(2);
            receivedMessage.Headers.First().ToUri.Should().Be(clientQueueId.ToMessageUri().ToString());
            receivedMessage.Headers.First().FromUri.Should().Be(identityQueueId.ToMessageUri().ToString());
            receivedMessage.Headers.Skip(1).First().Should().Be(header);
        }

        [Fact]
        public async Task GivenTwoNodes_UsingHostCall_MessagesArePassed()
        {
            var clientMessageReceiveQueue = new ConcurrentQueue<NetMessage>();

            var clientQueueId = new QueueId("default", "test", "clientNode");
            var identityQueueId = new QueueId("default", "test", "identityNode");

            IMessageRepository messageRepository = new MessageRepository(_application.GetMessageNetConfig(), _loggerFactory);
            await messageRepository.Unregister(clientQueueId, CancellationToken.None);
            await messageRepository.Unregister(identityQueueId, CancellationToken.None);

            IMessageNetHost netHost = null!;

            Func<NetMessage, Task> clientNodeReceiver = x =>
            {
                clientMessageReceiveQueue.Enqueue(x);
                return Task.CompletedTask;
            };

            Func<NetMessage, Task> identityNodeReceiver = async x =>
            {
                NetMessage netMessage = new NetMessageBuilder(x)
                    .Add(x.Header.WithReply("get.response"))
                    .Build();

                await netHost.Send(netMessage);
            };

            netHost = new MessageNetHostBuilder()
                .SetConfig(_application.GetMessageNetConfig())
                .SetRepository(new MessageRepository(_application.GetMessageNetConfig(), _loggerFactory))
                .SetAwaiter(new MessageAwaiterManager())
                .AddNodeReceiver(new NodeHostReceiver(identityQueueId, identityNodeReceiver))
                .AddNodeReceiver(new NodeHostReceiver(clientQueueId, clientNodeReceiver))
                .Build(_loggerFactory);

            await netHost.Start(CancellationToken.None);

            var header = new MessageHeader(identityQueueId.ToMessageUri(), clientQueueId.ToMessageUri(), "get");

            var message = new NetMessageBuilder()
                .Add(header)
                .Build();

            NetMessage receivedMessage = await netHost.Call(message);

            await netHost.Stop();

            receivedMessage.Headers.Count.Should().Be(2);
            receivedMessage.Headers.First().ToUri.Should().Be(clientQueueId.ToMessageUri().ToString());
            receivedMessage.Headers.First().FromUri.Should().Be(identityQueueId.ToMessageUri().ToString());
            receivedMessage.Headers.Skip(1).First().Should().Be(header);

            clientMessageReceiveQueue.Count.Should().Be(0);
        }

        [Fact]
        public async Task GivenTwoNodes_WhenSendingMultipleMessages_ShouldReceive()
        {
            const int max = 100;
            var clientMessageReceiveQueue = new ConcurrentQueue<NetMessage>();
            var messageReceivedQueue = new ConcurrentQueue<NetMessage>();

            var clientQueueId = new QueueId("default", "test", "clientNode");
            var identityQueueId = new QueueId("default", "test", "identityNode");

            IMessageRepository messageRepository = new MessageRepository(_application.GetMessageNetConfig(), _loggerFactory);
            await messageRepository.Unregister(clientQueueId, CancellationToken.None);
            await messageRepository.Unregister(identityQueueId, CancellationToken.None);

            IMessageNetHost netHost = null!;

            Func<NetMessage, Task> clientNodeReceiver = x =>
            {
                clientMessageReceiveQueue.Enqueue(x);
                return Task.CompletedTask;
            };

            Func<NetMessage, Task> identityNodeReceiver = async x =>
            {
                NetMessage netMessage = new NetMessageBuilder(x)
                    .Add(x.Header.WithReply("get.response"))
                    .Build();

                await netHost.Send(netMessage);
            };

            netHost = new MessageNetHostBuilder()
                .SetConfig(_application.GetMessageNetConfig())
                .SetRepository(new MessageRepository(_application.GetMessageNetConfig(), _loggerFactory))
                .SetAwaiter(new MessageAwaiterManager())
                .AddNodeReceiver(new NodeHostReceiver(identityQueueId, identityNodeReceiver))
                .AddNodeReceiver(new NodeHostReceiver(clientQueueId, clientNodeReceiver))
                .Build(_loggerFactory);

            await netHost.Start(CancellationToken.None);

            for (int index = 0; index < max; index++)
            {
                var header = new MessageHeader(identityQueueId.ToMessageUri(), clientQueueId.ToMessageUri(), $"get_{index}");

                var message = new NetMessageBuilder()
                    .Add(header)
                    .Build();

                NetMessage receivedMessage = await netHost.Call(message);
                messageReceivedQueue.Enqueue(receivedMessage);
            }

            await netHost.Stop();

            for (int index = 0; index < max; index++)
            {
                if (!messageReceivedQueue.TryDequeue(out NetMessage? receivedMessage)) throw new InvalidOperationException("empty queue");

                receivedMessage.Headers.Count.Should().Be(2);
                receivedMessage.Headers.First().ToUri.Should().Be(clientQueueId.ToMessageUri().ToString());
                receivedMessage.Headers.First().FromUri.Should().Be(identityQueueId.ToMessageUri().ToString());
                receivedMessage.Headers.Skip(1).First().Method.Should().Be($"get_{index}");
                receivedMessage.Headers.Skip(1).First().ToUri.Should().Be(identityQueueId.ToMessageUri().ToString());
                receivedMessage.Headers.Skip(1).First().FromUri.Should().Be(clientQueueId.ToMessageUri().ToString());
            }

            clientMessageReceiveQueue.Count.Should().Be(0);
        }
    }
}
