// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Khooversoft.Toolbox.Standard
{
    /// <summary>
    /// Fast deferred execution using lambda
    /// </summary>
    public class Deferred
    {
        private Action<IWorkContext> _execute;

        /// <summary>
        /// Construct with lambda to return value
        /// </summary>
        /// <param name="getValue"></param>
        public Deferred(Action<IWorkContext> execute)
        {
            execute.Verify(nameof(execute)).IsNotNull();

            _execute = x => InternalExecute(() => execute(x));
        }

        /// <summary>
        /// Return value (lazy)
        /// </summary>
        public void Execute(IWorkContext context) => _execute(context);

        /// <summary>
        /// Get value by switching lambda, will only be called once
        /// </summary>
        /// <param name="execute">lambda to get value</param>
        /// <returns>value</returns>
        private void InternalExecute(Action userAction)
        {
            Interlocked.Exchange(ref _execute, x => { });
            userAction();
        }
    }
}
