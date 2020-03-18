using FluentAssertions;
using Khooversoft.MessageNet.Host;
using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace MessageNet.Host.Tests
{
    [Collection("QueueTests")]
    public class ThreeNodeTests : IClassFixture<ApplicationFixture>
    {
        private readonly ITestOutputHelper _output;
        private const string _securityIdToken = "{securityJwtToken}";
        private const string _securityAuthToken = "{securityJwtAuthToken}";
        private readonly IWorkContext _workContext = WorkContextBuilder.Default;
        private readonly ApplicationFixture _application;
        private readonly MessageUri _clientUri = new QueueId("default", "test", "clientNode").ToMessageUri();
        private readonly MessageUri _identityUri = new QueueId("default", "test", "identityNode").ToMessageUri();
        private readonly MessageUri _authUri = new QueueId("default", "test", "authNode").ToMessageUri();
        private readonly MessageUri _tbankUri = new QueueId("default", "test", "tbankNode").ToMessageUri();
        private readonly ConcurrentQueue<NetMessage> _clientQueue = new ConcurrentQueue<NetMessage>();

        public ThreeNodeTests(ApplicationFixture application, ITestOutputHelper output)
        {
            _application = application;
            _output = output;
        }

        [Fact]
        public async Task GivenNodeSampleModel_WhenAuthFlowExecuted_ShouldPass()
        {
            var tcs = new TaskCompletionSource<bool>();

            _clientQueue.Clear();
            await DeleteAllQueues();

            IMessageNetHost client = await ClientNode(tcs);
            await IdentityNode(tcs);
            await AuthorizationNode(tcs);
            await TestBankNode(tcs);

            NetMessage netMessage = new NetMessageBuilder()
                .Add(new MessageHeader(_identityUri, _clientUri, "get.id.token"))
                .Add(MessageContent.Create(new IdentityTokenRequest { UserId = "user1@domain.com", SecurityToken = _securityIdToken }))
                .Build();

            await client.Send(_workContext, netMessage);

            tcs.Task.Wait(TimeSpan.FromSeconds(20));
        }

        private async Task<IMessageNetHost> ClientNode(TaskCompletionSource<bool> tcs)
        {
            IMessageNetHost netHost = null!;

            Func<NetMessage, Task> receiver = x => ProcessNetMessageReceived(x, _clientUri.ToString(), netHost, tcs);

            netHost = new MessageNetHostBuilder()
                .SetConfig(_application.GetMessageNetConfig())
                .SetRepository(new MessageRepository(_application.GetMessageNetConfig()))
                .SetAwaiter(new MessageAwaiterManager())
                .AddNodeReceiver(new NodeHostReceiver(_clientUri.ToQueueId(), receiver))
                .Build();

            await netHost.Start(_workContext);
            return netHost;
        }

        private async Task<IMessageNetHost> IdentityNode(TaskCompletionSource<bool> tcs)
        {
            IMessageNetHost netHost = null!;

            Func<NetMessage, Task> receiver = x => ProcessNetMessageReceived(x, _identityUri.ToString(), netHost, tcs);

            netHost = new MessageNetHostBuilder()
                .SetConfig(_application.GetMessageNetConfig())
                .SetRepository(new MessageRepository(_application.GetMessageNetConfig()))
                .SetAwaiter(new MessageAwaiterManager())
                .AddNodeReceiver(new NodeHostReceiver(_identityUri.ToQueueId(), receiver))
                .Build();

            await netHost.Start(_workContext);
            return netHost;
        }

        private async Task<IMessageNetHost> AuthorizationNode(TaskCompletionSource<bool> tcs)
        {
            IMessageNetHost netHost = null!;

            Func<NetMessage, Task> receiver = x => ProcessNetMessageReceived(x, _authUri.ToString(), netHost, tcs);

            netHost = new MessageNetHostBuilder()
                .SetConfig(_application.GetMessageNetConfig())
                .SetRepository(new MessageRepository(_application.GetMessageNetConfig()))
                .SetAwaiter(new MessageAwaiterManager())
                .AddNodeReceiver(new NodeHostReceiver(_authUri.ToQueueId(), receiver))
                .Build();

            await netHost.Start(_workContext);
            return netHost;
        }

        private async Task<IMessageNetHost> TestBankNode(TaskCompletionSource<bool> tcs)
        {
            IMessageNetHost netHost = null!;

            Func<NetMessage, Task> receiver = x => ProcessNetMessageReceived(x, _tbankUri.ToString(), netHost, tcs);

            netHost = new MessageNetHostBuilder()
                .SetConfig(_application.GetMessageNetConfig())
                .SetRepository(new MessageRepository(_application.GetMessageNetConfig()))
                .SetAwaiter(new MessageAwaiterManager())
                .AddNodeReceiver(new NodeHostReceiver(_tbankUri.ToQueueId(), receiver))
                .Build();

            await netHost.Start(_workContext);
            return netHost;
        }

        private async Task DeleteAllQueues()
        {
            _output.WriteLine("Deleting all queues");

            IMessageRepository messageRepository = new MessageRepository(_application.GetMessageNetConfig());
            await messageRepository.Unregister(_workContext, _clientUri.ToQueueId());
            await messageRepository.Unregister(_workContext, _identityUri.ToQueueId());
            await messageRepository.Unregister(_workContext, _authUri.ToQueueId());
            await messageRepository.Unregister(_workContext, _tbankUri.ToQueueId());
        }

        private Task ProcessNetMessageReceived(NetMessage netMessage, string toUri, IMessageNetHost netHost, TaskCompletionSource<bool> tcs)
        {
            netMessage.Header.ToUri.Should().Be(toUri);

            _output.WriteLine($"NetMessage id: :{netMessage.Header.MessageId}, toUri: {toUri}, method: {netMessage.Header.Method}");

            switch (netMessage.Header.Method)
            {
                // client -> identity
                case "get.id.token":
                    {
                        IdentityTokenRequest request = netMessage.Content.Deserialize<IdentityTokenRequest>();
                        request.Should().NotBeNull();
                        request.UserId.Should().Be("user1@domain.com");
                        request.SecurityToken.Should().Be(_securityIdToken);

                        NetMessage response = new NetMessageBuilder(netMessage)
                            .Add(netMessage.Header.WithReply("get.id.token.response"))
                            .Add(MessageContent.Create(new IdentityToken { JwtId = _securityIdToken }))
                            .Build();

                        return netHost.Send(_workContext, response);
                    }

                // identity -> client
                case "get.id.token.response":
                    {
                        IdentityToken response = netMessage.Content.Deserialize<IdentityToken>();
                        response.JwtId.Should().Be(_securityIdToken);

                        NetMessage message = new NetMessageBuilder(netMessage)
                            .Add(new MessageHeader(_authUri, _clientUri, "get.auth"))
                            .Add(MessageContent.Create(new ResourceAuthorizationRequest { JwtId = _securityIdToken, Resource = "get.balance" }))
                            .Build();

                        return netHost.Send(_workContext, message);
                    }

                // client -> auth
                case "get.auth":
                    {
                        ResourceAuthorizationRequest request = netMessage.Content.Deserialize<ResourceAuthorizationRequest>();
                        request.Should().NotBeNull();
                        request.JwtId.Should().Be(_securityIdToken);
                        request.Resource.Should().Be("get.balance");

                        NetMessage response = new NetMessageBuilder(netMessage)
                            .Add(netMessage.Header.WithReply("get.auth.response"))
                            .Add(MessageContent.Create(new ResourceAuthorization { JwtId = _securityIdToken, JwtAuth = _securityAuthToken }))
                            .Build();

                        return netHost.Send(_workContext, response);
                    }

                // client -> tbank
                case "get.auth.response":
                    {
                        ResourceAuthorization response = netMessage.Content.Deserialize<ResourceAuthorization>();
                        response.JwtId.Should().Be(_securityIdToken);
                        response.JwtAuth.Should().Be(_securityAuthToken);

                        NetMessage message = new NetMessageBuilder(netMessage)
                            .Add(new MessageHeader(_tbankUri, _clientUri, "get.balance", new MessageClaim("accountId", "1234"), new MessageClaim("bearer", response.JwtAuth)))
                            .Build();

                        return netHost.Send(_workContext, message);
                    }

                // tbank => client
                case "get.balance":
                    {
                        netMessage.HasClaim(new MessageClaim("bearer", _securityAuthToken)).Should().BeTrue();
                        MessageClaim accountId = netMessage.GetClaim("accountId").FirstOrDefault();
                        accountId.Should().NotBeNull();

                        var bankBalanceDetails = new TBankBalanceDetails
                        {
                            AccountId = accountId.Value,
                            Balance = 100.55m,
                        };

                        NetMessage message = new NetMessageBuilder(netMessage)
                            .Add(new MessageHeader(_tbankUri, _clientUri, "get.balance.response", new MessageClaim("accountId", "1234")))
                            .Add(MessageContent.Create(bankBalanceDetails))
                            .Build();

                        return netHost.Send(_workContext, message);
                    }

                // client (last response)
                case "get.balance.response":
                    {
                        TBankBalanceDetails response = netMessage.Content.Deserialize<TBankBalanceDetails>();
                        response.Should().NotBeNull();
                        response.AccountId.Should().Be("1234");
                        response.Balance.Should().Be(100.55m);

                        tcs.SetResult(true);
                        return Task.CompletedTask;
                    }

                default:
                    throw new InvalidOperationException("unknown method");
            }
        }

        private class IdentityTokenRequest
        {
            public string UserId { get; set; }

            public string SecurityToken { get; set; }
        }

        private class IdentityToken
        {
            public string JwtId { get; set; }
        }

        private class ResourceAuthorizationRequest
        {
            public string JwtId { get; set; }

            public string Resource { get; set; }
        }

        public class ResourceAuthorization
        {
            public string JwtId { get; set; }

            public string JwtAuth { get; set; }
        }

        public class TBankBalanceDetails
        {
            public string AccountId { get; set; }

            public decimal Balance { get; set; }
        }
    }
}
