using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Standard
{
    public class MessageRouter<T>
    {
        private readonly List<Func<T, Task<bool>>> _routeFunctions = new List<Func<T, Task<bool>>>();
        private readonly object _lock = new object();
        private List<Func<T, Task<bool>>>? _cacheList;

        public MessageRouter()
        {
        }

        public int Count => _routeFunctions.Count;

        public MessageRouter<T> Clear()
        {
            _cacheList = null;

            lock (_lock)
            {
                _routeFunctions.Clear();
            }

            return this;
        }

        public MessageRouter<T> Add(Func<T, Task<bool>> routeFunction)
        {
            routeFunction.Verify(nameof(routeFunction)).IsNotNull();
            _cacheList = null;

            lock (_lock)
            {
                _routeFunctions.Add(routeFunction);
                return this;
            }
        }

        public async Task Post(T message)
        {
            lock (_lock)
            {
                _cacheList ??= _routeFunctions.ToList();
            }

            foreach (var item in _routeFunctions)
            {
                bool okay = await item(message);
                if (!okay) return;
            }
        }
    }
}
