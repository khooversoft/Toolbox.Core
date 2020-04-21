using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Toolbox.Run
{
    public interface IProperty : IEnumerable<KeyValuePair<string, object?>>
    {
        void Clear();

        void Set<T>(T value);

        void Set<T>(string name, T value);

        T Get<T>();

        T Get<T>(string name);

        bool TryGetValue<T>(out T value);

        bool TryGetValue<T>(string name, out T value);

        bool Exist<T>();

        bool Exist(string name);
    }
}
