using Khooversoft.Toolbox.Standard;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace KHooversoft.Toolbox.Dataflow
{
    internal class TopicController
    {
        private readonly BufferBlock<byte[]> _buffer;
        private readonly BroadcastBlock<byte[]> _broadcast;
        private readonly ConcurrentDictionary<string, SubscriptionRegistration> _subscriptions = new ConcurrentDictionary<string, SubscriptionRegistration>(StringComparer.OrdinalIgnoreCase);
        private readonly ILogger _logger;

        public TopicController(string topic, ILogger logger)
        {
            topic.VerifyNotEmpty(nameof(topic));

            Topic = topic;
            _logger = logger;
            _buffer = new BufferBlock<byte[]>();
            _broadcast = new BroadcastBlock<byte[]>(x => x);
            _buffer.LinkTo(_broadcast, new DataflowLinkOptions { PropagateCompletion = true });
        }

        public string Topic { get; }

        public ITargetBlock<byte[]> TargetSync => _buffer;

        public ITopicClient CreateClient() => new TopicClient(Topic, TargetSync);

        public ITopicSubscription CreateSubscription(string name, Action<byte[]> sync)
        {
            name.VerifyNotEmpty(nameof(name));
            sync.VerifyNotNull(nameof(sync));

            _logger.LogTrace($"Creating subscription");

            var subscription = new TopicSubscription(Topic, name, sync, x => ReleaseSubscription(x.Name));
            IDisposable release = _broadcast.LinkTo(subscription.TargetSync, new DataflowLinkOptions { PropagateCompletion = true });

            _subscriptions[subscription.Name] = new SubscriptionRegistration(subscription, release);
            return subscription;
        }

        public async Task Close()
        {
            _logger.LogInformation($"Closing topic controller");

            _buffer.Complete();

            await _buffer.Completion;
            await _broadcast.Completion;

            KeyValuePair<string, SubscriptionRegistration>[] currentRegistration = _subscriptions.ToArray();
            _subscriptions.Clear();

            currentRegistration
                .ForEach(x => x.Value.Subscription.TargetSync.Complete());

            Task[] tasks = currentRegistration
                .Select(x => x.Value.Subscription.TargetSync.Completion)
                .ToArray();

            _subscriptions.Clear();

            await Task.WhenAll(tasks);
        }

        private void ReleaseSubscription(string name)
        {
            if (_subscriptions.TryGetValue(name, out SubscriptionRegistration subscriptionRegistration))
            {
                _logger.LogInformation($"Release subscription {name}");
                subscriptionRegistration.ReleaseLink.Dispose();
            }
        }

        private struct SubscriptionRegistration
        {
            public SubscriptionRegistration(TopicSubscription subscription, IDisposable releaseLink)
            {
                Subscription = subscription.VerifyNotNull(nameof(subscription));
                ReleaseLink = releaseLink.VerifyNotNull(nameof(releaseLink));
            }

            public TopicSubscription Subscription { get; }

            public IDisposable ReleaseLink { get; }
        }
    }
}
