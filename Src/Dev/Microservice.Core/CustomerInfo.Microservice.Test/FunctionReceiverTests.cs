using Autofac;
using CustomerInfo.Microservice.Test.Application;
using Khooversoft.Toolbox.Standard;
using MicroserviceHost;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CustomerInfo.Microservice.Test
{
    public class FunctionReceiverTests : IClassFixture<ApplicationFixture>
    {
        private readonly ApplicationFixture _application;

        public FunctionReceiverTests(ApplicationFixture application)
        {
            _application = application;
        }


        [Fact]
        public async Task GivenFunction_AfterBind_SendSingleMessageIsReceived()
        {
            var tokenSource = new CancellationTokenSource();

            IWorkContext workContext = new WorkContextBuilder()
                .Set(tokenSource.Token)
                .Build();

            Task run = RunFunctions(workContext);

        }

        private async Task RunFunctions(IWorkContext workContext)
        {
            IExecutionContext executionContext = new MicroserviceHost.ExecutionContext();
            IContainer container = new ContainerBuilder().Build();

            var loadAssembly = new LoadAssemblyActivity(_application.Option);
            await loadAssembly.Load(workContext, executionContext);

            using ILifetimeScope lifetimeScope = container.BeginLifetimeScope();
            var buildContainerActivity = new BuildContainerActivity(lifetimeScope);
            await buildContainerActivity.Build(workContext, executionContext);

            var runFunctionReceivers = new RunFunctionReceiversActivity(_application.Option);
            await runFunctionReceivers.Run(workContext, executionContext);
        }

        public async Task RunReceiver()
        {
            var clientQueueId = new QueueId("default", "test", "clientNode");
            var identityQueueId = new QueueId("default", "test", "identityNode");

            IMessageRepository messageRepository = new MessageRepository(_application.GetMessageNetConfig());
            await messageRepository.Unregister(_workContext, clientQueueId);
            await messageRepository.Unregister(_workContext, identityQueueId);

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

                await netHost.Send(_workContext, netMessage);
            };

            netHost = new MessageNetHostBuilder()
                .SetConfig(_application.GetMessageNetConfig())
                .SetRepository(new MessageRepository(_application.GetMessageNetConfig()))
                .SetAwaiter(new MessageAwaiterManager())
                .AddNodeReceiver(new NodeHostReceiver(identityQueueId, identityNodeReceiver))
                .AddNodeReceiver(new NodeHostReceiver(clientQueueId, clientNodeReceiver))
                .Build();

            await netHost.Start(_workContext);

            var header = new MessageHeader(identityQueueId.ToMessageUri(), clientQueueId.ToMessageUri(), "get");

            var message = new NetMessageBuilder()
                .Add(header)
                .Build();

            await netHost.Send(_workContext, message);

            NetMessage receivedMessage = await clientReceiverTask.Task;

            receivedMessage.Headers.Count.Should().Be(2);
            receivedMessage.Headers.First().ToUri.Should().Be(clientQueueId.ToMessageUri().ToString());
            receivedMessage.Headers.First().FromUri.Should().Be(identityQueueId.ToMessageUri().ToString());
            receivedMessage.Headers.Skip(1).First().Should().Be(header);
        }
    }
}
