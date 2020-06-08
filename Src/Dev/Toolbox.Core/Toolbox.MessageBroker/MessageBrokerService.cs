using Khooversoft.Toolbox.Standard;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.MessageBroker
{
    public class MessageBrokerService
    {
        private readonly ILogger<MessageBrokerService> _logger;
        private ConcurrentDictionary<string, TopicController> _topics = new ConcurrentDictionary<string, TopicController>(StringComparer.OrdinalIgnoreCase);

        public MessageBrokerService(ILogger<MessageBrokerService> logger)
        {
            _logger = logger.VerifyNotNull(nameof(logger));
        }

        public void CreateTopic(string topic)
        {
            topic.VerifyNotEmpty(nameof(topic));

            _logger.LogTrace($"Creating topic {topic}");
            _topics.TryAdd(topic, new TopicController(topic));
        }

        public ITopicClient CreateClient(string topic)
        {
            topic.VerifyNotEmpty(nameof(topic));

            _logger.LogTrace($"Creating client for topic {topic}");

            _topics.TryGetValue(topic, out TopicController topicRegistered)
                .VerifyAssert<bool, KeyNotFoundException>(x => x == true, _ => $"Topic {topic} not registered");

            return new TopicClient(topic, topicRegistered.TargetSync);
        }

        public ITopicSubscription CreateSubscription(string topic, Action<byte[]> sync)
        {
            sync.VerifyNotNull(nameof(sync));

            _topics.TryGetValue(topic, out TopicController value)
                .VerifyAssert(x => true, _ => $"Topic {topic} does not exist");

            return value.CreateSubscription(sync);
        }

        public IReadOnlyList<string> Topics => _topics
            .ToArray()
            .Select(x => x.Value.Topic)
            .ToArray();

        public async Task Stop()
        {
            ConcurrentDictionary<string, TopicController>? topic = Interlocked.Exchange(ref _topics, null!);
            if (topic != null)
            {
                foreach(var item in topic.Values)
                {
                    await item.DisposeAsync();
                }
            }
        }
    }
}
