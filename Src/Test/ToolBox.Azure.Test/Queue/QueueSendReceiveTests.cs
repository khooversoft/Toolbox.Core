using FluentAssertions;
using Khoover.Toolbox.TestTools;
using Khooversoft.Toolbox.Azure;
using Khooversoft.Toolbox.Standard;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ToolBox.Azure.Test.Queue
{
    [Collection("QueueTests")]
    public class QueueSendReceiveTests
    {
        private readonly ILoggerFactory _loggerFactory = new TestLoggerFactory();
        private readonly AzureTestOption _testOption;

        public QueueSendReceiveTests()
        {
            _testOption = new TestOptionBuilder().Build();
        }

        //[Fact(Skip = "IntegrationTests")]
        [Fact]
        public async Task GivenQueue_WhenMessageSent_ShouldReceive()
        {
            const int max = 10;

            var testMessageReceived = new ConcurrentQueue<TestMessage>();
            var sendMessage = new List<TestMessage>();

            var completionSource = new TaskCompletionSource<bool>();
            var tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(100));
            tokenSource.Token.Register(() => completionSource.SetResult(false));

            Func<TestMessage, Task> receiverFunc = x =>
            {
                testMessageReceived.Enqueue(x);
                if (testMessageReceived.Count >= max) completionSource.SetResult(true);
                return Task.FromResult(0);
            };

            var queueDefinition = new QueueDefinition("QueueSendReceiveTests1");
            await CreateQueue(queueDefinition);

            IQueueManagement queue = _testOption.GetQueueManagement(_loggerFactory);

            var sender = new MessageSender(queue.ConnectionString, queueDefinition.QueueName);
            var receiver = new QueueReceiver<TestMessage>(queue.ConnectionString, queueDefinition.QueueName, _loggerFactory.CreateLogger<QueueReceiver<TestMessage>>());

            await receiver.Start(receiverFunc);

            await Enumerable.Range(0, max)
                .Select(x => new TestMessage(x, $"Value_{x}"))
                .Select(x => { sendMessage.Add(x); return x; })
                .Select(x => new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(x))))
                .ForEachAsync(async x => await sender.SendAsync(x));

            bool result = await completionSource.Task;
            result.Should().BeTrue("timed out");

            await receiver.Stop();

            testMessageReceived.Count.Should().Be(max);

            testMessageReceived
                .Zip(sendMessage, (o, i) => (o, i))
                .All(x => x.o.Index == x.i.Index && x.o.Value == x.i.Value)
                .Should().BeTrue();

            await DeleteQueue(queueDefinition);
        }

        //[Fact(Skip = "IntegrationTests")]
        [Fact]
        public async Task GivenTwoQueue_MessageRoundTrip_ShouldSuccess()
        {
            const int max = 10;

            var testMessageReceived = new ConcurrentQueue<TestMessage>();
            var sendMessage = new List<TestMessage>();

            var completionSource = new TaskCompletionSource<bool>();
            var tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(100));
            tokenSource.Token.Register(() => completionSource.SetResult(false));

            Func<TestMessage, Task> receiverFunc = x =>
            {
                testMessageReceived.Enqueue(x);
                if (testMessageReceived.Count >= max) completionSource.SetResult(true);
                return Task.FromResult(0);
            };

            var queue1Definition = new QueueDefinition("QueueSendReceiveTests1");
            var queue2Definition = new QueueDefinition("QueueSendReceiveTests2");

            await CreateQueue(queue1Definition);
            await CreateQueue(queue2Definition);

            var bounceReceiverTask = new TaskCompletionSource<bool>();
            Task bounce = Bounce(queue1Definition, queue2Definition, bounceReceiverTask);

            IQueueManagement queue = _testOption.GetQueueManagement(_loggerFactory);

            var sender = new MessageSender(queue.ConnectionString, queue2Definition.QueueName);
            var receiver = new QueueReceiver<TestMessage>(queue.ConnectionString, queue1Definition.QueueName, _loggerFactory.CreateLogger<QueueReceiver<TestMessage>>());

            await receiver.Start(receiverFunc);

            await Enumerable.Range(0, max)
                .Select(x => new TestMessage(x, $"Value_{x}"))
                .Select(x => { sendMessage.Add(x); return x; })
                .Select(x => new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(x))))
                .ForEachAsync(async x => await sender.SendAsync(x));

            bool result = await completionSource.Task;
            result.Should().BeTrue("timed out");

            await receiver.Stop();
            bounceReceiverTask.SetResult(true);

            testMessageReceived.Count.Should().Be(max);

            testMessageReceived
                .Zip(sendMessage, (o, i) => (o, i))
                .All(x => x.o.Index == x.i.Index && x.o.Value == x.i.Value)
                .Should().BeTrue();

            await DeleteQueue(queue1Definition);
            await DeleteQueue(queue2Definition);
        }

        private async Task Bounce(QueueDefinition sendDefinition, QueueDefinition receiverDefinition, TaskCompletionSource<bool> finish)
        {
            IQueueManagement queue = _testOption.GetQueueManagement(_loggerFactory);

            var sender = new MessageSender(queue.ConnectionString, sendDefinition.QueueName);
            int count = 0;

            Func<TestMessage, Task> receiverFunc = async x =>
            {
                count++;
                await sender.SendAsync(new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(x))));
            };

            var receiver = new QueueReceiver<TestMessage>(queue.ConnectionString, receiverDefinition.QueueName, _loggerFactory.CreateLogger<QueueReceiver<TestMessage>>());
            await receiver.Start(receiverFunc);

            await finish.Task;
            await receiver.Stop();
        }

        private async Task CreateQueue(QueueDefinition queueDefinition)
        {
            await DeleteQueue(queueDefinition);

            IQueueManagement queue = _testOption.GetQueueManagement(_loggerFactory);

            await queue.Create(queueDefinition, CancellationToken.None);
        }

        private async Task DeleteQueue(QueueDefinition queueDefinition)
        {
            IQueueManagement queue = _testOption.GetQueueManagement(_loggerFactory);

            if ((await queue.Exist(queueDefinition.QueueName, CancellationToken.None)))
            {
                await queue.Delete(queueDefinition.QueueName, CancellationToken.None);
            }
        }

        private class TestMessage
        {
            public TestMessage(int index, string value)
            {
                Index = index;
                Value = value;
            }

            public int Index { get; }

            public string Value { get; }
        }
    }
}
