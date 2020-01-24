using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MicroserviceHost
{
    internal class RunFunctionReceivers : IAction
    {
        private readonly IOption _option;

        public RunFunctionReceivers(IOption option)
        {
            _option = option;
        }

        public Task Run(IWorkContext context, IExecutionContext executionContext)
        {
            context.Verify(nameof(context)).IsNotNull();
            executionContext.Verify(nameof(executionContext)).IsNotNull();

            //IReadOnlyList<FunctionMessageReceiver> receivers = executionContext.FunctionConfigurations
            //    .Select(x => new FunctionMessageReceiver(x))
            //    .ToList();

            // Start receivers
            //await receivers
            //    .Select(x => x.Start(context.WithNewCv()))
            //    .WhenAll();

            ManualResetEventSlim waitFor = new ManualResetEventSlim();

            context.Telemetry.Info(context, "Waiting for cancellation event");
            context.CancellationToken.Register(() => waitFor.Set());
            waitFor.Wait();

            context.Telemetry.Info(context, "Shutting down receivers");
            //await receivers
            //    .ForEachAsync(x => x.Stop(context));

            return Task.CompletedTask;
        }
    }
}
