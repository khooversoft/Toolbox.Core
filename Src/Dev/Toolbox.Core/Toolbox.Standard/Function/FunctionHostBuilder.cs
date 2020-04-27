using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Standard
{
    public class FunctionHostBuilder
    {
        private readonly IList<FunctionInfo> _functionInfos = new List<FunctionInfo>();

        public FunctionHostBuilder() { }

        public IReadOnlyList<FunctionInfo> Functions => (IReadOnlyList<FunctionInfo>)_functionInfos;

        public IServiceContainer? Container { get; set; }

        public FunctionHostBuilder AddFunction(params FunctionInfo[] functionInfos)
        {
            functionInfos
                .ForEach(x => x.VerifyAssert(y => isTask(y.MethodInfo.ReturnType), $"{x.MethodInfo.Name} does not return Task"));

            functionInfos
                .ForEach(_functionInfos.Add);

            return this;

            static bool isTask(Type type) => type == typeof(Task) || type.IsSubclassOf(typeof(Task));
        }

        public FunctionHostBuilder UseContainer(IServiceContainer serviceContainer)
        {
            serviceContainer.VerifyNotNull(nameof(serviceContainer));

            Container = serviceContainer;
            return this;
        }

        public FunctionHostBuilder LoadAssemblyFunctions<TAttr>(string assemblyFile) where TAttr : Attribute
        {
            Reflection.LoadFromAssemblyPath(assemblyFile)
                .FindMethodsByAttribute<TAttr>()
                .ForEach(_functionInfos.Add);

            return this;
        }

        public FunctionHost Build()
        {
            Functions.VerifyNotNull(nameof(Functions));

            return new FunctionHost(Functions, Container);
        }
    }
}
