using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroserviceHost
{
    public class ExecutionContext : IExecutionContext
    {
        public ExecutionContext(IEnumerable<FunctionConfiguration> functionConfigurations)
        {
            functionConfigurations.VerifyNotNull(nameof(functionConfigurations));

            FunctionConfigurations = functionConfigurations.ToList();
        }

        public IReadOnlyList<FunctionConfiguration> FunctionConfigurations { get; }
    }
}
