using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Khooversoft.Toolbox.MessageBroker
{
    public class TopicController : IAsyncDisposable
    {
        private BufferBlock<byte[]> _buffer;
        private readonly BroadcastBlock<byte[]> _broadcast;
        private readonly Dictionary<Guid, (TopicSubscription subscription, IDisposable releaseLink)> _subscriptions = new Dictionary<Guid, (TopicSubscription subscription, IDisposable releaseLink)>();
        private readonly object _lock = new object();

        public TopicController(string topic)
        {
            topic.VerifyNotEmpty(nameof(topic));

            Topic = topic;
            _buffer = new BufferBlock<byte[]>();
            _broadcast = new BroadcastBlock<byte[]>(x => x);
            _buffer.LinkTo(_broadcast, new DataflowLinkOptions { PropagateCompletion = true });
        }

        public string Topic { get; }

        public ITargetBlock<byte[]> TargetSync => _buffer;

        public ITopicClient CreateClient()
        {
            _buffer.VerifyNotNull($"Topic {Topic} has been disposed");

            return new TopicClient(Topic, TargetSync);
        }

        public ITopicSubscription CreateSubscription(Action<byte[]> sync)
        {
            _buffer.VerifyNotNull($"Topic {Topic} has been disposed");

            lock (_lock)
            {
                var subscription = new TopicSubscription(Topic, sync, x => ReleaseSubscription(x));
                IDisposable release = _broadcast.LinkTo(subscription.TargetSync, new DataflowLinkOptions { PropagateCompletion = true });

                _subscriptions[subscription.SubscriptionKey] = (subscription, release);
                return subscription;
            }
        }

        public async ValueTask DisposeAsync()
        {
            BufferBlock<byte[]> buffer = Interlocked.Exchange(ref _buffer, null!);
            if (buffer != null)
            {
                buffer.Complete();
                _broadcast.Complete();

                await buffer.Completion;
                await _broadcast.Completion;
            }

            foreach (var item in _subscriptions.Values)
            {
                item.releaseLink.Dispose();
                await item.subscription.DisposeAsync();
            }

            _subscriptions.Clear();
        }

        private void ReleaseSubscription(Guid subscriptionKey)
        {
            lock (_lock)
            {
                if (_subscriptions.TryGetValue(subscriptionKey, out (TopicSubscription subscription, IDisposable releaseLink) subscriptionRegistration))
                {
                    subscriptionRegistration.releaseLink.Dispose();
                }
            }
        }
    }
}
