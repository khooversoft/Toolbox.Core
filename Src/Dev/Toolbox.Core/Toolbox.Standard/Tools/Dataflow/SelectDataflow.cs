// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

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
            transform.VerifyNotNull(nameof(transform));

            Transform = transform;
            Predicate = x => true;
        }

        public SelectDataflow(Func<T, T> transform, Func<T, bool> predicate)
            : this(transform)
        {
            predicate.VerifyNotNull(nameof(predicate));

            Predicate = predicate;
        }

        public Func<T, T> Transform { get; }

        public Func<T, bool> Predicate { get; }
    }
}
