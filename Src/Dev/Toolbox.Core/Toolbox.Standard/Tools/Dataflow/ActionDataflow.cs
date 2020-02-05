using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Khooversoft.Toolbox.Standard
{
    public class ActionDataflow<T> : IDataflow<T>
    {
        public ActionDataflow(Action<T> action)
        {
            action.Verify(nameof(action)).IsNotNull();
            Action = action;
            Predicate = x => true;
        }

        public ActionDataflow(Action<T> action, Func<T, bool> predicate)
            : this(action)
        {
            predicate.Verify(nameof(predicate)).IsNotNull();

            Predicate = predicate;
        }

        public Action<T> Action { get; }

        public Func<T, bool> Predicate { get; }
    }
}
