// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Standard
{
    /// <summary>
    /// State item used in a state manager
    /// </summary>
    public interface IStateItem
    {
        /// <summary>
        /// Name of state item
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Ignore error for set operation
        /// </summary>
        bool IgnoreError { get; }

        /// <summary>
        /// Test state
        /// </summary>
        /// <param name="context">context</param>
        /// <returns>true state is active, false if not</returns>
        Task<bool> Test(IWorkContext context);

        /// <summary>
        /// Set state to make "Test" true
        /// </summary>
        /// <param name="context">context</param>
        /// <returns>true if set, false if not</returns>
        Task<bool> Set(IWorkContext context);
    }
}
