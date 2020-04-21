// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Khooversoft.Toolbox.Standard
{
    /// <summary>
    /// State manager builder
    /// </summary>
    public class StateManagerBuilder : IEnumerable<IStateItem>
    {
        public StateManagerBuilder() { }

        public IList<IStateItem> StateItems { get; set; } = new List<IStateItem>();

        public RetryPolicy? Policy { get; set; }

        public StateManagerBuilder Add(params IStateItem[] item)
        {
            item.ForEach(x => StateItems.Add(x));
            return this;
        }

        public StateManagerBuilder AddPolicy(RetryPolicy? policy)
        {
            Policy = policy;
            return this;
        }

        public IStateManager Build()
        {
            StateItems.VerifyNotNull(nameof(StateItems));

            return new StateManager(Policy, StateItems.ToArray());
        }

        public IEnumerator<IStateItem> GetEnumerator() => StateItems.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => StateItems.GetEnumerator();
    }
}
