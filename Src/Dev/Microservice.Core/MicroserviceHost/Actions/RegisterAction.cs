using Khooversoft.MessageNet.Client;
using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroserviceHost.Actions
{
    internal class RegisterAction : IAction
    {
        private readonly IOption _option;

        public RegisterAction(IOption option)
        {
            _option = option;
        }

        public Task Run(IWorkContext context, IExecutionContext executionContext)
        {
            context.Verify(nameof(context)).IsNotNull();
            executionContext.Verify(nameof(executionContext)).IsNotNull();

            //var functionConfigurations = executionContext.Functions
            //    .Select(x => MessageNetFactory(x))
            //    .ToList();

            //executionContext.SetFunctionConfigurations(functionConfigurations);
            return Task.CompletedTask;
        }

        private FunctionConfiguration MessageNetFactory(Function function)
        {
            string nodeId = function.FunctionAttribute.NodeId
                .Resolve(_option.Properties)
                .Verify(nameof(function.FunctionAttribute.NodeId))
                .IsNotEmpty($"Function: {function.Name} {function.FunctionAttribute.NodeId} is required")
                .Assert(x => x.GetPropertyNames()?.Count == 0, $"Unresolved properties for {function.FunctionAttribute.NodeId}")
                .Value;

            return new FunctionConfiguration(function, new Uri(_option.NameServerUri), _option.ServiceBusConnection, nodeId);

            //function.FunctionConfiguration(new MessageNetClient(new Uri(_option.NameServerUri), x => _option.ServiceBusConnection), nodeId);
            //return function;
        }
    }
}
