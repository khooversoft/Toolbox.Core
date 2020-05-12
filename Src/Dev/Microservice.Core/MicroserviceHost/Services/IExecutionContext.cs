using Autofac;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;

namespace MicroserviceHost
{
    internal interface IExecutionContext
    {
        ILifetimeScope? LifetimeScope { get; set; }

        IReadOnlyList<FunctionInfo>? FunctionInfos { get; set; }

        IReadOnlyList<Type>? KnownInjectMethodTypes { get; set; }
    }
}