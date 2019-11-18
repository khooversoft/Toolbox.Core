using Khooversoft.Toolbox.Standard;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessageHub.Management
{
    public class RegisterStore : IRegisterStore
    {
        private readonly ConcurrentDictionary<string, QueueRegistration> _data = new ConcurrentDictionary<string, QueueRegistration>(StringComparer.OrdinalIgnoreCase);

        public RegisterStore()
        {
        }

        public int Count => _data.Count;

        public void Clear(IWorkContext context) => _data.Clear();

        public void Set(IWorkContext context, QueueRegistration queueRegistration)
        {
            queueRegistration.Verify(nameof(queueRegistration)).IsNotNull();
            queueRegistration.NodeId.Verify(nameof(queueRegistration.NodeId)).IsNotNull();

            _data.AddOrUpdate(queueRegistration.NodeId, queueRegistration, (x, r) => queueRegistration);
        }

        public QueueRegistration Remove(IWorkContext context, string path)
        {
            path.Verify(nameof(path)).IsNotEmpty();

            _data.Remove(path, out QueueRegistration queueRegistration);
            return queueRegistration;
        }

        public bool TryGet(string path, out QueueRegistration queueRegistration)
        {
            path.Verify(nameof(path)).IsNotNull();

            return _data.TryGetValue(path, out queueRegistration);
        }

        public IEnumerator<KeyValuePair<string, QueueRegistration>> GetEnumerator()
        {
            return _data.ToList().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _data.ToList().GetEnumerator();
        }
    }
}
