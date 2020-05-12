using Khooversoft.MessageNet.Host;
using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Autofac;
using Khooversoft.Toolbox.Standard;
using Microservice.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MicroserviceHost
{
    internal class RunFunctionReceiversActivity
    {
        private readonly IOption _option;

        public RunFunctionReceiversActivity(IOption option)
        {
            _option = option;
        }

        public async Task Run(IWorkContext context, IExecutionContext executionContext)
        {
            context.VerifyNotNull(nameof(context));
            executionContext.VerifyNotNull(nameof(executionContext));

            IReadOnlyList<IFunction> functions = BuildFunctionHost(executionContext);
            IMessageNetHost messageNetHost = BuildMessageNetHost(context, executionContext, functions);

            var cancelAwaiter = new TaskCompletionSource<bool>();

            context.Telemetry.Info(context, "Waiting for cancellation event");
            context.CancellationToken.Register(() => cancelAwaiter.SetResult(true));

            await messageNetHost.Start(context);
            await cancelAwaiter.Task;

            context.Telemetry.Info(context, "Shutting down receivers");
            await messageNetHost.Stop(context);
        }

        private IReadOnlyList<IFunction> BuildFunctionHost(IExecutionContext executionContext)
        {
            FunctionHost functionHost = new FunctionHostBuilder()
                .UseContainer(new ServiceContainerBuilder().SetLifetimeScope(executionContext.LifetimeScope!).Build())
                .AddFunction(executionContext.FunctionInfos.ToArray())
                .Build();

            return functionHost.GetFunctions();
        }

        private IMessageNetHost BuildMessageNetHost(IWorkContext context, IExecutionContext executionContext, IReadOnlyList<IFunction> functions)
        {
            var messageFunctions = functions
                .Select(x => new MessageFunction(
                    name: x.FunctionInfo.Name,
                    nodeId: x.FunctionInfo.Attribute.CastAs<MessageFunctionAttribute>().NodeId.Resolve(_option.Properties),
                    function: x,
                    messageType: x.FunctionInfo.MethodInfo.GetMissingParameters(executionContext.KnownInjectMethodTypes.ToArray()).First(),
                    telemetry: context.Telemetry,
                    context: context
                    ))
                .ToList();

            var netHostBuilder = new MessageNetHostBuilder()
                .SetConfig(_option.MessageNetConfig)
                .SetRepository(new MessageRepository(_option.MessageNetConfig))
                .SetAwaiter(new MessageAwaiterManager());

            messageFunctions
                .ForEach(x => netHostBuilder.AddNodeReceiver(new NodeHostReceiver(QueueId.Parse(x.NodeId), x.Receiver)));

            return netHostBuilder.Build();
        }
    }
}
