using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks.Dataflow;
using Xunit;

namespace Toolbox.Standard.Test.Tools
{
    public class PipelineBlockTests
    {
        [Fact]
        public void GivenBroadcastBlock_WhenSendMessage_ShouldReceiveMessage()
        {
            int count = 0;

            var jobs = new BroadcastBlock<string>(x => x)
                .RouteTo(x => count++);
        }

        internal static class BlockExtensions
        {
            public BroadcastBlock<T> RouteTo<T>(this BroadcastBlock<T> block, Action<T> action, Func<T, bool> predicate)
            {
                var actionBlock = new ActionBlock<T>(action);
                block.LinkTo(actionBlock, predicate);
            }
        }
    }
}
