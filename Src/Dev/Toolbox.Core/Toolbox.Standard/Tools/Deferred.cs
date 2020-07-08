// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Khooversoft.Toolbox.Standard
{
    /// <summary>
    /// Fast deferred execution using lambda, will only execute action once
    /// </summary>
    public class Deferred
    {
        private Action _execute;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="execute">execute one time</param>
        public Deferred(Action execute)
        {
            execute.VerifyNotNull(nameof(execute));

            _execute = () =>
            {
                Interlocked.Exchange(ref _execute, () => { });
                execute();
            };
        }

        /// <summary>
        /// Return value (lazy)
        /// </summary>
        public void Execute() => _execute();
    }
}
