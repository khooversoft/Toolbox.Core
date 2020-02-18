// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Standard
{
    public class StateManager : IStateManager
    {
        public StateManager(StateManagerBuilder builder)
        {
            builder.Verify(nameof(builder)).IsNotNull();

            StateItems = new List<IStateItem>(builder.StateItems);
        }

        public IReadOnlyList<IStateItem> StateItems { get; }

        /// <summary>
        /// Execute Set the work item(s) in the plan
        /// </summary>
        /// <param name="context">work context</param>
        /// <returns>true if passed, false if not</returns>
        public async Task<bool> Set(IWorkContext context)
        {
            context.Verify(nameof(context)).IsNotNull();

            try
            {
                context.Telemetry.Verbose(context, "Running state plan");

                foreach (var item in StateItems)
                {
                    context.Telemetry.Verbose(context, $"Executing state plan 'Test' for item {item.Name}");

                    bool testResult = await item.Test(context).ConfigureAwait(false);

                    context.CancellationToken.ThrowIfCancellationRequested();
                    context.Telemetry.Verbose(context, $"Executed state plan 'Test' for item {item.Name} with {testResult} result");

                    if (!testResult)
                    {
                        context.Telemetry.Verbose(context, $"Executing state plan 'Set' for item {item.Name}");

                        testResult = await item.Set(context).ConfigureAwait(false);

                        context.CancellationToken.ThrowIfCancellationRequested();
                        context.Telemetry.Verbose(context, $"Executed state plan 'Set' for item {item.Name} with {testResult} result");

                        if (!testResult && !item.IgnoreError)
                        {
                            context.Telemetry.Verbose(context, $"State plan did not completed successfully, name={item.Name}");
                            return false;
                        }
                    }
                }

                context.Telemetry.Verbose(context, "State plan completed successfully");
                return true;
            }
            finally
            {
                context.Telemetry.Verbose(context, "State plan exited");
            }
        }

        /// <summary>
        /// Test state of items
        /// </summary>
        /// <param name="context">work context</param>
        /// <returns>true if all pass, false if not</returns>
        public async Task<bool> Test(IWorkContext context)
        {
            context.Verify(nameof(context)).IsNotNull();

            try
            {
                context.Telemetry.Verbose(context, "Running state plan");

                foreach (var item in StateItems)
                {
                    context.Telemetry.Verbose(context, $"Executing state plan 'Test' for item {item.Name}");

                    bool result = await item.Test(context).ConfigureAwait(false);

                    context.CancellationToken.ThrowIfCancellationRequested();
                    context.Telemetry.Verbose(context, $"Executed state plan 'Test' for item {item.Name} with {result} result");

                    if (!result)  return false;
                }

                context.Telemetry.Verbose(context, "State plan completed successfully");
                return true;
            }
            finally
            {
                context.Telemetry.Verbose(context, "State plan exited");
            }
        }

        public StateManagerBuilder ToBuilder()
        {
            return new StateManagerBuilder(this);
        }
    }
}
