using FluentAssertions;
using Khooversoft.Toolbox.Standard;
using KHooversoft.Toolbox.Dataflow;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Toolbox.Dataflow.Test.Application;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Toolbox.Dataflow.Test.MessageBroker
{
    public class MessageBrokerTests
    {
        private readonly TestLoggingFactory _testLoggingFactory;
        private readonly ILogger _logger;

        public MessageBrokerTests(ITestOutputHelper testOutput)
        {
            _testLoggingFactory = new TestLoggingFactory(testOutput);
            _logger = _testLoggingFactory.CreateLogger<MessageBrokerTests>();
        }

        [Fact]
        public async Task GivenActionAndSubjsction_WhenMessageSent_AllAreReceived()
        {
            BufferBlock<int> bufferBlock = new BufferBlock<int>();
            BroadcastBlock<int> broadcastBlock = new BroadcastBlock<int>(x => x);
            bufferBlock.LinkTo(broadcastBlock, new DataflowLinkOptions { PropagateCompletion = true });

            var queue = new ConcurrentQueue<(int, int)>();

            const int subscriptionCount = 100;
            ActionBlock<int>[] receivers = Enumerable.Range(0, subscriptionCount)
                .Select(x =>
                {
                    var block = new ActionBlock<int>(y => queue.Enqueue((x, y)));
                    broadcastBlock.LinkTo(block, new DataflowLinkOptions { PropagateCompletion = true });
                    return block;
                }).ToArray();

            const int max = 100;
            Task[] bufferSendTask = Enumerable.Range(0, max)
                .Select(x => bufferBlock.SendAsync(x))
                .ToArray();

            await Task.WhenAll(bufferSendTask);

            bufferBlock.Complete();

            Task[] tasks = new Task[]
            {
                bufferBlock.Completion,
                broadcastBlock.Completion
            }.Concat(receivers.Select(x => x.Completion))
            .ToArray();

            await Task.WhenAll(tasks);

            queue.Count.Should().Be(max * subscriptionCount);
        }

        [Fact]
        public async Task GivenTopicSub_WhenMessageSent_Receives()
        {
            const string topic = "Main";
            const string data = "this is data";

            var broker = new MessageBrokerService(_testLoggingFactory.CreateLogger<MessageBrokerService>());
            var receiveQueue = new Queue<byte[]>();

            broker.CreateTopic(topic);

            ITopicSubscription topicSubscription = broker.CreateSubscription(topic, "Sub1", x => receiveQueue.Enqueue(x));
            ITopicClient topicClient = broker.CreateClient(topic);

            byte[] sourceData = Encoding.UTF8.GetBytes(data);
            await topicClient.SendAsync(sourceData);

            await broker.Stop();

            receiveQueue.Count.Should().Be(1);
            Enumerable.SequenceEqual(sourceData, receiveQueue.Dequeue()).Should().BeTrue();
        }

        [Fact]
        public async Task GivenTopicSub_WhenMultipleMessagesSent_Receives()
        {
            const string topic = "Main";
            const int max = 10;

            IReadOnlyList<byte[]> sources = Enumerable.Range(0, max)
                .Select(x => Encoding.UTF8.GetBytes($"Data_{x}"))
                .ToList();

            var broker = new MessageBrokerService(_testLoggingFactory.CreateLogger<MessageBrokerService>());
            var receiveQueue = new Queue<byte[]>();

            broker.CreateTopic(topic);

            ITopicSubscription topicSubscription = broker.CreateSubscription(topic, "Sub1", x => receiveQueue.Enqueue(x));
            ITopicClient topicClient = broker.CreateClient(topic);

            await sources.ForEachAsync(async x => await topicClient.SendAsync(x));

            await broker.Stop();

            _logger.LogInformation("Assert");
            receiveQueue.Count.Should().Be(max);

            foreach (var item in sources)
            {
                Enumerable.SequenceEqual(item, receiveQueue.Dequeue()).Should().BeTrue();
            }
        }

        [Fact]
        public async Task GivenTopicTwoSub_WhenMultipleMessagesSent_Receives()
        {
            const string topic = "Main";
            const int max = 10;

            IReadOnlyList<byte[]> sources = Enumerable.Range(0, max)
                .Select(x => Encoding.UTF8.GetBytes($"Data_{x}"))
                .ToList();

            var broker = new MessageBrokerService(_testLoggingFactory.CreateLogger<MessageBrokerService>());
            var receiveQueue1 = new Queue<byte[]>();
            var receiveQueue2 = new Queue<byte[]>();

            broker.CreateTopic(topic);

            broker.CreateSubscription(topic, "Sub1", x => receiveQueue1.Enqueue(x));
            broker.CreateSubscription(topic, "Sub2", x => receiveQueue2.Enqueue(x));
            ITopicClient topicClient = broker.CreateClient(topic);

            await sources.ForEachAsync(async x => await topicClient.SendAsync(x));

            await broker.Stop();

            _logger.LogInformation("Assert");
            receiveQueue1.Count.Should().Be(max);
            receiveQueue2.Count.Should().Be(max);

            foreach (var item in sources)
            {
                Enumerable.SequenceEqual(item, receiveQueue1.Dequeue()).Should().BeTrue();
                Enumerable.SequenceEqual(item, receiveQueue2.Dequeue()).Should().BeTrue();
            }
        }

        [Fact]
        public async Task GivenTopicTwoSub_WhenMultipleMessagesSent_OneStopHalf_Receives()
        {
            const string topic = "Main";
            const int max = 10;

            IReadOnlyList<byte[]> sources = Enumerable.Range(0, max)
                .Select(x => Encoding.UTF8.GetBytes($"Data_{x}"))
                .ToList();

            _logger.LogInformation("Starting");
            var broker = new MessageBrokerService(_testLoggingFactory.CreateLogger<MessageBrokerService>());
            var receiveQueue1 = new Queue<byte[]>();
            var receiveQueue2 = new Queue<byte[]>();

            broker.CreateTopic(topic);

            ITopicSubscription sub1 = broker.CreateSubscription(topic, "Sub1", x => receiveQueue1.Enqueue(x));

            broker.CreateSubscription(topic, "Sub2", x => receiveQueue2.Enqueue(x));

            ITopicClient topicClient = broker.CreateClient(topic);

            _logger.LogInformation("Sending bytes");

            await sources
                .Take(max / 2)
                .ForEachAsync(async x => await topicClient.SendAsync(x));

            sub1.Dispose();

            await sources
                .Skip(max / 2)
                .ForEachAsync(async x => await topicClient.SendAsync(x));

            await broker.Stop();

            receiveQueue1.Count.Should().Be(max / 2);
            receiveQueue2.Count.Should().Be(max);
        }

        [Fact]
        public async Task GivenTwoTopicTwoSub_WhenMultipleMessagesSent_Receives()
        {
            const int max = 10;

            var topics = new[]
            {
                new
                {
                    Topic = "Main1",
                    Data = Enumerable.Range(0, max).Select(x => Encoding.UTF8.GetBytes($"Main1_data_{x}")).ToList(),
                    Queue = new Queue<byte[]>(),
                },
                new
                {
                    Topic = "Main2",
                    Data = Enumerable.Range(0, max).Select(x => Encoding.UTF8.GetBytes($"Main2_data_{x}")).ToList(),
                    Queue = new Queue<byte[]>(),
                },
            };

            var broker = new MessageBrokerService(_testLoggingFactory.CreateLogger<MessageBrokerService>());

            topics.ForEach(x =>
            {
                broker.CreateTopic(x.Topic);
                broker.CreateSubscription(x.Topic, "Sub1", y => x.Queue.Enqueue(y));
            });

            var clients = topics.Select(x => broker.CreateClient(x.Topic));

            foreach (var item in topics.Zip(clients, (o, i) => (Topic: o, Client: i)))
            {
                foreach (var data in item.Topic.Data)
                {
                    await item.Client.SendAsync(data);
                }
            }

            await broker.Stop();

            foreach (var item in topics)
            {
                foreach (var data in item.Data)
                {
                    Enumerable.SequenceEqual(data, item.Queue.Dequeue()).Should().BeTrue();
                }
            }
        }
    }
}
