using Khooversoft.Toolbox.Standard;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KHooversoft.Toolbox.Dataflow
{
    public class MessageBrokerService
    {
        private readonly ILogger<MessageBrokerService> _logger;
        private readonly ConcurrentDictionary<string, TopicController> _topics = new ConcurrentDictionary<string, TopicController>(StringComparer.OrdinalIgnoreCase);

        public MessageBrokerService(ILogger<MessageBrokerService> logger)
        {
            _logger = logger.VerifyNotNull(nameof(logger));
        }

        public void CreateTopic(string topic)
        {
            topic.VerifyNotEmpty(nameof(topic));

            _logger.LogTrace($"Creating topic {topic}");
            _topics.TryAdd(topic, new TopicController(topic, _logger));
        }

        public ITopicClient CreateClient(string topic)
        {
            topic.VerifyNotEmpty(nameof(topic));

            _logger.LogTrace($"Creating client for topic {topic}");

            _topics.TryGetValue(topic, out TopicController topicRegistered)
                .VerifyAssert<bool, KeyNotFoundException>(x => x == true, _ => $"Topic {topic} not registered");

            return new TopicClient(topic, topicRegistered.TargetSync);
        }

        public ITopicSubscription CreateSubscription(string topic, string name, Action<byte[]> sync)
        {
            sync.VerifyNotNull(nameof(sync));

            _logger.LogTrace($"Creating subscription {topic}");

            _topics.TryGetValue(topic, out TopicController value)
                .VerifyAssert(x => true, _ => $"Topic {topic} does not exist");

            return value.CreateSubscription(name, sync);
        }

        public IReadOnlyList<string> Topics => _topics
            .ToArray()
            .Select(x => x.Value.Topic)
            .ToArray();

        public async Task Stop()
        {
            _logger.LogTrace($"Stopping service");

            KeyValuePair<string, TopicController>[] topics = _topics.ToArray();
            _topics.Clear();

            foreach (var item in topics)
            {
                Trace.WriteLine($"Stopping topic {item.Key}");
                await item.Value.Close();
            }
        }
    }
}
