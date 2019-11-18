using Khooversoft.Toolbox.Standard;
using System.Collections.Generic;

namespace MessageHub.Management
{
    public interface IRegisterStore : IEnumerable<KeyValuePair<string, QueueRegistration>>
    {
        int Count { get; }

        void Set(IWorkContext context, QueueRegistration queueRegistration);
        void Clear(IWorkContext context);
        QueueRegistration Remove(IWorkContext context, string path);
        bool TryGet(string path, out QueueRegistration queueRegistration);
    }
}