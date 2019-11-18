// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;

namespace Khooversoft.Toolbox.Standard
{
    /// <summary>
    /// State manager builder
    /// </summary>
    public class StateManagerBuilder : IEnumerable<IStateItem>
    {
        public StateManagerBuilder()
        {
            StateItems = new List<IStateItem>();
        }

        public StateManagerBuilder(StateManager workPlan)
        {
            workPlan.Verify(nameof(workPlan)).IsNotNull();

            StateItems = new List<IStateItem>(workPlan.StateItems);
        }

        public IList<IStateItem> StateItems { get; }

        public StateManagerBuilder Add(IStateItem item)
        {
            item.Verify(nameof(item)).IsNotNull();

            StateItems.Add(item);
            return this;
        }

        public StateManagerBuilder Add(IEnumerable<IStateItem> items)
        {
            items.Verify(nameof(items)).IsNotNull();

            items.ForEach(x => Add(x));
            return this;
        }

        public IStateManager Build()
        {
            return new StateManager(this);
        }

        public IEnumerator<IStateItem> GetEnumerator()
        {
            return StateItems.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return StateItems.GetEnumerator();
        }
    }
}
