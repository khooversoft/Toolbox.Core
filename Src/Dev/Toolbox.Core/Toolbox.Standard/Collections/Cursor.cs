// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Khooversoft.Toolbox.Standard
{
    public class Cursor<T>
    {
        private int _cursor = -1;
        private readonly IList<T> _list;

        public Cursor(IList<T> collection)
        {
            collection.Verify(nameof(collection)).IsNotNull();

            _list = collection;
        }

        public IReadOnlyList<T> List => (List<T>)_list;

        public int Index { get => _cursor; set => _cursor = Math.Max(Math.Min(value, _list.Count), -1); }

        public T Current => _cursor >= 0 && _cursor < _list.Count ? _list[_cursor] : default;

        public bool IsCursorAtEnd => _cursor >= 0 && _cursor < _list.Count ? false : true;

        public void Reset() => _cursor = -1;

        public bool TryNext(out T value)
        {
            value = default!;
            if (_list.Count == 0) return false;

            int current = Math.Min(Interlocked.Increment(ref _cursor), _list.Count);
            if (current >= _list.Count) return false;

            value = _list[current];
            return true;
        }

        public bool TryPeek(out T value)
        {
            value = default!;
            if (_list.Count == 0) return false;

            int current = Math.Min(_cursor + 1, _list.Count);
            if (current >= _list.Count) return false;

            value = _list[current];
            return true;
        }
    }
}
