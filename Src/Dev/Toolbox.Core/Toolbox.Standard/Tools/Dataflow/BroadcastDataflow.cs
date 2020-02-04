using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Khooversoft.Toolbox.Standard
{
    public class BroadcastDataflow<T> : IDataflow<T>, IEnumerable<IDataflow<T>>
    {
        private readonly IList<IDataflow<T>> _targets = new List<IDataflow<T>>();

        public BroadcastDataflow() { }

        public Task Post(T message)
        {
            return Task.CompletedTask;
        }

        public BroadcastDataflow<T> Add(IDataflow<T> targetBlock)
        {
            _targets.Add(targetBlock);
            return this;
        }

        public IEnumerator<IDataflow<T>> GetEnumerator() => _targets.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _targets.GetEnumerator();

        public static BroadcastDataflow<T> operator +(BroadcastDataflow<T> self, BroadcastDataflow<T> subject)
        {
            self.Add(subject);
            return self;
        }
    }
}
