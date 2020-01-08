using Khooversoft.MessageNet.Client;
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

            var functions = new List<Function>();

            executionContext.Functions
                .Select(x => MessageNetFactory(x));

        }

        private Function MessageNetFactory(Function function)
        {
            string nodeId = function.FunctionAttribute.InputUri
                .Resolve(_option.Properties)
                .Verify(nameof(function.FunctionAttribute.InputUri))
                .IsNotEmpty($"Function: {function.Name} {function.FunctionAttribute.InputUri} is required")
                .Value;

            return function.SetMessageNetClient(new MessageNetClient(nodeId, _option.NameServerUri, ))
        }
    }
}
