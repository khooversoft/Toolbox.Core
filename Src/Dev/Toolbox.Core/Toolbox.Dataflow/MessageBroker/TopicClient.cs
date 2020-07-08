using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace KHooversoft.Toolbox.Dataflow
{
    internal class TopicClient : ITopicClient
    {
        private readonly ITargetBlock<byte[]> _targetBlock;

        public TopicClient(string topic, ITargetBlock<byte[]> targetBlock)
        {
            Topic = topic.VerifyNotEmpty(nameof(topic));
            _targetBlock = targetBlock.VerifyNotNull(nameof(targetBlock));
        }

        public string Topic { get; }

        public void Post(byte[] message) => _targetBlock.Post(message);

        public Task SendAsync(byte[] message) => _targetBlock.SendAsync(message);
    }
}
