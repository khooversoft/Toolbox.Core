// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

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
            action.VerifyNotNull(nameof(action));
            Action = action;
            Predicate = x => true;
        }

        public ActionDataflow(Action<T> action, Func<T, bool> predicate)
            : this(action)
        {
            predicate.VerifyNotNull(nameof(predicate));

            Predicate = predicate;
        }

        public Action<T> Action { get; }

        public Func<T, bool> Predicate { get; }
    }
}
