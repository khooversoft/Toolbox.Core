using System.Collections.Generic;

namespace MicroserviceHost
{
    internal interface IExecutionContext
    {
        IReadOnlyList<Function> Functions { get; }

        void SetFunctions(IEnumerable<Function> functions);
    }
}