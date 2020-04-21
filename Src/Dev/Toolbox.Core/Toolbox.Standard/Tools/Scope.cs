// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Khooversoft.Toolbox.Standard
{
    /// <summary>
    /// Constructs a disposable object for finalization of object or operation
    /// </summary>
    /// <typeparam name="T">value type</typeparam>
    public sealed class Scope<T> : IDisposable
    {
        private Action<T>? _action;

        public Scope(T value, Action<T> disposeAction)
        {
            disposeAction.VerifyNotNull(nameof(disposeAction));

            _action = disposeAction;
            Value = value;
        }

        public T Value { get; }

        public void Dispose()
        {
            Action<T>? action = Interlocked.Exchange(ref _action, null);
            action?.Invoke(Value);
        }
    }
}
