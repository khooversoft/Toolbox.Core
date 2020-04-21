// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Standard
{
    public class StateManager : IStateManager
    {
        private IReadOnlyList<IStateItem> _stateItems;
        private readonly RetryPolicy? _policy;

        public StateManager(params IStateItem[] stateItems)
        {
            _stateItems = stateItems.ToList();
        }

        public StateManager(RetryPolicy? policy, params IStateItem[] stateItems)
        {
            _policy = policy;
            _stateItems = stateItems.ToList();
        }

        /// <summary>
        /// Execute Set the work item(s) in the plan
        /// </summary>
        /// <param name="context">work context</param>
        /// <returns>true if passed, false if not</returns>
        public async Task<bool> Set(IWorkContext context)
        {
            context.VerifyNotNull(nameof(context));

            foreach (var item in _stateItems)
            {
                if (_policy == null)
                {
                    bool state = await ExecuteState(context, item);
                    if (!state) return state;
                }

                await _policy!.Execute(async () => await ExecuteState(context, item));
            }

            return true;
        }

        /// <summary>
        /// Test state of items
        /// </summary>
        /// <param name="context">work context</param>
        /// <returns>true if all pass, false if not</returns>
        public async Task<bool> Test(IWorkContext context)
        {
            context.VerifyNotNull(nameof(context));

            foreach (var item in _stateItems)
            {
                bool result = await item.Test(context).ConfigureAwait(false);
                context.CancellationToken.ThrowIfCancellationRequested();

                if (!result) return false;
            }

            return true;
        }

        /// <summary>
        /// Execute state functions, test and set
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="stateItem">state item</param>
        /// <returns>true if okay, false if not</returns>
        private async Task<bool> ExecuteState(IWorkContext context, IStateItem stateItem)
        {
            context.CancellationToken.ThrowIfCancellationRequested();
            bool testResult = await stateItem.Test(context).ConfigureAwait(false);

            if (!testResult)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                testResult = await stateItem.Set(context).ConfigureAwait(false);

                if (!testResult && !stateItem.IgnoreError) return false;
            }

            return true;
        }
    }
}
