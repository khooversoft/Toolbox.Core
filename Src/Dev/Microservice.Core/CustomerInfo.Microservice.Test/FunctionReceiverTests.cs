using Autofac;
using CustomerInfo.Microservice.Test.Application;
using FluentAssertions;
using Khooversoft.MessageNet.Host;
using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Standard;
using MicroserviceHost;
using System;
using System.Linq;
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
            var tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(1));

            IWorkContext workContext = new WorkContextBuilder()
                .Set(tokenSource.Token)
                .Build();

            Task runReceiver = RunFunctions(workContext);
            Task runSender = RunSender(workContext);

            tokenSource.Cancel();

            await Task.WhenAll(runReceiver, runSender);
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

        public async Task RunSender(IWorkContext workContext)
        {
            var clientQueueId = new QueueId("default", "test", "Get-CustomerInfo");
            var sourceId = new QueueId("default", "test", "asker");

            IMessageNetHost netHost = null!;

            var clientReceiverTask = new TaskCompletionSource<NetMessage>();

            Func<NetMessage, Task> clientNodeReceiver = x =>
            {
                clientReceiverTask.SetResult(x);
                return Task.CompletedTask;
            };

            netHost = new MessageNetHostBuilder()
                .SetConfig(_application.Option.MessageNetConfig)
                .SetRepository(new MessageRepository(_application.Option.MessageNetConfig))
                .SetAwaiter(new MessageAwaiterManager())
                .AddNodeReceiver(new NodeHostReceiver(clientQueueId, clientNodeReceiver))
                .Build();

            await netHost.Start(workContext);

            var header = new MessageHeader(sourceId.ToMessageUri(), clientQueueId.ToMessageUri(), "get");

            var message = new NetMessageBuilder()
                .Add(header)
                .Build();

            NetMessage response = await netHost.Call(workContext, message);

            workContext.CancellationToken.Register(() => clientReceiverTask.SetException(new OperationCanceledException()));

            NetMessage receivedMessage = await clientReceiverTask.Task;

            receivedMessage.Headers.Count.Should().Be(2);
            receivedMessage.Headers.First().ToUri.Should().Be(clientQueueId.ToMessageUri().ToString());
            receivedMessage.Headers.First().FromUri.Should().Be(sourceId.ToMessageUri().ToString());
            receivedMessage.Headers.Skip(1).First().Should().Be(header);
        }
    }
}
