using Autofac;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroserviceHost
{
    internal class BuildContainerActivity
    {
        private readonly ILifetimeScope _lifetimeScope;

        public BuildContainerActivity(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }

        public Task Build(IWorkContext context, IExecutionContext executionContext)
        {
            executionContext.VerifyNotNull(nameof(executionContext));

            executionContext.LifetimeScope = _lifetimeScope.BeginLifetimeScope(builder =>
            {
                executionContext.FunctionInfos
                    .Select(x => x.MethodInfo.DeclaringType)
                    .GroupBy(x => x.FullName, (k, t) => t.First())
                    .ForEach(x => builder.RegisterType(x!));

                executionContext.FunctionInfos
                    .Select(x => GetMessageParameterType(x, executionContext))
                    .ForEach(x => builder.RegisterType(x));
            });

            return Task.CompletedTask;
        }

        private Type GetMessageParameterType(FunctionInfo function, IExecutionContext executionContext)
        {
            Type[] missingTypes = function.MethodInfo.GetMissingParameters(executionContext.KnownInjectMethodTypes.ToArray());
            missingTypes.Length.VerifyAssert(x => x == 1, $"Only 1 unknown parameter can be used for function {function.Name} to receive message");

            return missingTypes[0];
        }
    }
}
