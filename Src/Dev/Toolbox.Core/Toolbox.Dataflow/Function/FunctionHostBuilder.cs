using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace KHooversoft.Toolbox.Dataflow
{
    /// <summary>
    /// Build a function host
    /// </summary>
    public class FunctionHostBuilder
    {
        private readonly IList<FunctionInfo> _functionInfos = new List<FunctionInfo>();

        public FunctionHostBuilder() { }

        public IReadOnlyList<FunctionInfo> Functions => (IReadOnlyList<FunctionInfo>)_functionInfos;

        public Func<Type, object>? CreateFactory { get; set; }

        public FunctionHostBuilder AddFunction(params FunctionInfo[] functionInfos)
        {
            functionInfos
                .ForEach(x => x.VerifyAssert(y => isTask(y.MethodInfo.ReturnType), $"{x.MethodInfo.Name} does not return Task"));

            functionInfos
                .ForEach(_functionInfos.Add);

            return this;

            static bool isTask(Type type) => type == typeof(Task) || type.IsSubclassOf(typeof(Task));
        }

        public FunctionHostBuilder UseContainer(Func<Type, object> createFactory)
        {
            createFactory.VerifyNotNull(nameof(createFactory));

            CreateFactory = createFactory;
            return this;
        }

        public FunctionHostBuilder LoadAssemblyFunctions<TAttr>(string assemblyFile) where TAttr : Attribute
        {
            ReflectionTools.LoadFromAssemblyPath(assemblyFile)
                .FindMethodsByAttribute<TAttr>()
                .ForEach(_functionInfos.Add);

            return this;
        }

        public FunctionHost Build()
        {
            Functions.VerifyNotNull(nameof(Functions));

            return new FunctionHost(Functions, CreateFactory);
        }
    }
}
