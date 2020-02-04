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
            Action = action;
        }

        public Action<T> Action { get; }
    }
}
