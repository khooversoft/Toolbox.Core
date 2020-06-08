using Khooversoft.Toolbox.Standard;
using Microsoft.VisualStudio.TestPlatform.Common.Utilities;
using System;
using Xunit;
using Khooversoft.Toolbox.MessageBroker;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using FluentAssertions;
using System.Linq;

namespace Toolbox.MessageBroker.Test
{
    public class MessageBrokerTests
    {
        [Fact]
        public async Task GivenTopicSub_WhenMessageSent_Receives()
        {
            const string topic = "Main";
            const string data = "this is data";

            var logger = new MemoryLogger();
            var broker = new MessageBrokerService(logger.CreateLogger<MessageBrokerService>());
            var receiveQueue = new Queue<byte[]>();

            broker.CreateTopic(topic);

            ITopicSubscription topicSubscription = broker.CreateSubscription(topic, x => receiveQueue.Enqueue(x));
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

            var logger = new MemoryLogger();
            var broker = new MessageBrokerService(logger.CreateLogger<MessageBrokerService>());
            var receiveQueue = new Queue<byte[]>();

            broker.CreateTopic(topic);

            ITopicSubscription topicSubscription = broker.CreateSubscription(topic, x => receiveQueue.Enqueue(x));
            ITopicClient topicClient = broker.CreateClient(topic);

            await sources.ForEachAsync(async x => await topicClient.SendAsync(x));
            await broker.Stop();

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

            var logger = new MemoryLogger();
            var broker = new MessageBrokerService(logger.CreateLogger<MessageBrokerService>());
            var receiveQueue1 = new Queue<byte[]>();
            var receiveQueue2 = new Queue<byte[]>();

            broker.CreateTopic(topic);

            broker.CreateSubscription(topic, x => receiveQueue1.Enqueue(x));
            broker.CreateSubscription(topic, x => receiveQueue2.Enqueue(x));
            ITopicClient topicClient = broker.CreateClient(topic);

            await sources.ForEachAsync(async x => await topicClient.SendAsync(x));
            await broker.Stop();

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

            var logger = new MemoryLogger();
            var broker = new MessageBrokerService(logger.CreateLogger<MessageBrokerService>());
            var receiveQueue1 = new Queue<byte[]>();
            var receiveQueue2 = new Queue<byte[]>();

            broker.CreateTopic(topic);

            ITopicSubscription sub1 = broker.CreateSubscription(topic, x => receiveQueue1.Enqueue(x));
            broker.CreateSubscription(topic, x => receiveQueue2.Enqueue(x));
            ITopicClient topicClient = broker.CreateClient(topic);

            await sources.Take(max / 2).ForEachAsync(async x => await topicClient.SendAsync(x));
            await sub1.DisposeAsync();

            await sources.Skip(max / 2).ForEachAsync(async x => await topicClient.SendAsync(x));
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

            var logger = new MemoryLogger();
            var broker = new MessageBrokerService(logger.CreateLogger<MessageBrokerService>());

            topics.ForEach(x =>
            {
                broker.CreateTopic(x.Topic);
                broker.CreateSubscription(x.Topic, y => x.Queue.Enqueue(y));
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
