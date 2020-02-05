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
            Predicate = x => true;
        }

        public SelectDataflow(Func<T, T> transform, Func<T, bool> predicate)
            : this(transform)
        {
            predicate.Verify(nameof(predicate)).IsNotNull();

            Predicate = predicate;
        }

        public Func<T, T> Transform { get; }

        public Func<T, bool> Predicate { get; }
    }
}
