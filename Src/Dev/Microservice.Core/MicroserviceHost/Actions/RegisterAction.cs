using Khooversoft.Toolbox.Standard;
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
            context.VerifyNotNull(nameof(context));
            executionContext.VerifyNotNull(nameof(executionContext));

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
                .VerifyNotEmpty($"Function: {function.Name} {function.FunctionAttribute.NodeId} is required")
                .VerifyAssert(x => x.GetPropertyNames()?.Count == 0, $"Unresolved properties for {function.FunctionAttribute.NodeId}");

            return new FunctionConfiguration(function, nodeId);
        }
    }
}
