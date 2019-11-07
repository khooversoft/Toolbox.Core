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
            disposeAction.Verify(nameof(disposeAction)).IsNotNull();

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
