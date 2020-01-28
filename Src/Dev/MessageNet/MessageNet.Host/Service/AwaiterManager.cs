using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MessageNet.Host
{
    internal class AwaiterManager : IAwaiterManager
    {
        private readonly ConcurrentDictionary<string, TaskCompletionSource<bool>> _completion = new ConcurrentDictionary<string, TaskCompletionSource<bool>>(StringComparer.OrdinalIgnoreCase);

        public AwaiterManager() { }

        public AwaiterManager Add(string id, TaskCompletionSource<bool> task)
        {
            id.Verify(nameof(id)).IsNotEmpty();
            task.Verify(nameof(task)).IsNotNull();

            _completion[id] = task;
            return this;
        }

        public AwaiterManager SetResult(string id, bool state)
        {
            id.Verify(nameof(id)).IsNotEmpty();

            TaskCompletionSource<bool> tcs;

            if (_completion.TryRemove(id, out tcs!))
            {
                tcs.SetResult(state);
            }

            return this;
        }
    }
}
