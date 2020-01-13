using System.Collections.Generic;

namespace MicroserviceHost
{
    public interface IExecutionContext
    {
        IReadOnlyList<FunctionConfiguration> FunctionConfigurations { get; }
    }
}