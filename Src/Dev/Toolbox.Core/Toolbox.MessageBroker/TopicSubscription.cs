using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Khooversoft.Toolbox.MessageBroker
{
    public class TopicSubscription : ITopicSubscription
    {
        private ActionBlock<byte[]> _queue;
        private Action<Guid> _release;

        public TopicSubscription(string topic, Action<byte[]> sync, Action<Guid> release)
        {
            topic.VerifyNotEmpty(nameof(topic));
            sync.VerifyNotNull(nameof(sync));
            release.VerifyNotNull(nameof(release));

            Topic = topic;
            _release = release;
            _queue = new ActionBlock<byte[]>(sync);
        }

        public Guid SubscriptionKey { get; } = Guid.NewGuid();

        public ITargetBlock<byte[]> TargetSync => _queue;

        public string Topic { get; }

        public async ValueTask DisposeAsync()
        {
            ActionBlock<byte[]> queue = Interlocked.Exchange(ref _queue, null!);
            if (queue != null)
            {
                queue.Complete();
                await queue.Completion;
            }

            Interlocked.Exchange(ref _release, null!)?.Invoke(SubscriptionKey);
        }
    }
}
