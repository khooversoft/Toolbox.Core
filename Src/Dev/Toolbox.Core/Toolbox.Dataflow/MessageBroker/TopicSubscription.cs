using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace KHooversoft.Toolbox.Dataflow
{
    internal class TopicSubscription : ITopicSubscription
    {
        private ActionBlock<byte[]> _queue;
        private Action<TopicSubscription> _release;

        public TopicSubscription(string topic, string name, Action<byte[]> sync, Action<TopicSubscription> release)
        {
            topic.VerifyNotEmpty(nameof(topic));
            sync.VerifyNotNull(nameof(sync));
            release.VerifyNotNull(nameof(release));

            Topic = topic;
            Name = name;
            _release = release;

            _queue = new ActionBlock<byte[]>(sync);
        }

        public Guid SubscriptionKey { get; } = Guid.NewGuid();

        public ITargetBlock<byte[]> TargetSync => _queue;

        public string Topic { get; }

        public string Name { get; }

        public void Dispose() => Interlocked.Exchange(ref _release, null!)?.Invoke(this);
    }
}
