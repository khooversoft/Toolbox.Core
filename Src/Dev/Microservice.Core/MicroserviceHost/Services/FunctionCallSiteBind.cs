using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroserviceHost
{
    internal class FunctionCallSiteBind
    {
        public FunctionCallSiteBind(Function function)
        {
            Function = function;
        }

        public object Instance { get; private set; } = null!;

        public Function Function { get; }

        public void Bind(IWorkContext context)
        {
            CreateInstance(context);
            SetupReceiver(context);
        }

        private void CreateInstance(IWorkContext context)
        {
            Instance.Verify(nameof(Instance)).Assert(x => x == null, "Function has already been binded");

            Type declaringType = Function.MethodInfo.DeclaringType!
                .Verify(nameof(Function.MethodInfo.DeclaringType))
                .IsNotNull()
                .Value;

            context.Container.Verify(nameof(context.Container)).IsNotNull("No container");

            Instance = context.Container!.GetService(declaringType);
        }

        private void SetupReceiver(IWorkContext context)
        {
            //Function.MethodInfo.Invoke(Instance, )
        }
    }
}
