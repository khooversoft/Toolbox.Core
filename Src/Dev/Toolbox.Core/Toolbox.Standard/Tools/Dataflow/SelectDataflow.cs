using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks.Dataflow;

namespace Khooversoft.Toolbox.Standard
{
    public class SelectDataflow<T> : IDataflow<T>
    {
        public SelectDataflow(Func<T, T> transform)
        {
            transform.Verify(nameof(transform)).IsNotNull();

            Transform = transform;
        }

        public Func<T, T> Transform { get; }
    }
}
