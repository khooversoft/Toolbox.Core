using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MessageNet.Host
{
    internal class AwaiterManager
    {
        private readonly ConcurrentDictionary<Guid, TaskCompletionSource<NetMessage>> _completion = new ConcurrentDictionary<Guid, TaskCompletionSource<NetMessage>>();

        public AwaiterManager() { }

        public AwaiterManager Add(Guid id, TaskCompletionSource<NetMessage> task)
        {
            task.Verify(nameof(task)).IsNotNull();

            _completion[id] = task;
            return this;
        }

        public AwaiterManager SetResult(NetMessage? netMessage)
        {
            if (netMessage == null!) return this;

            TaskCompletionSource<NetMessage> tcs;

            if (_completion.TryRemove(netMessage.Header.MessageId, out tcs!))
            {
                tcs.SetResult(netMessage);
            }

            return this;
        }
    }
}
