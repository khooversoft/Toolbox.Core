using Khooversoft.Toolbox.Standard;
using System.Threading.Tasks;

namespace KHooversoft.Toolbox.Dataflow
{
    /// <summary>
    /// Invoke a function's method
    /// </summary>
    public class FunctionInvoke : IFunction
    {
        public FunctionInvoke(object instance, FunctionInfo functionInfo)
        {
            Instance = instance;
            FunctionInfo = functionInfo;
        }

        public object Instance { get; }

        public FunctionInfo FunctionInfo { get; }

        public Task<TReturn> Invoke<TReturn>(params object[] parameters) => FunctionInfo.MethodInfo.InvokeAsync<TReturn>(Instance, parameters);

        public Task Invoke(params object[] parameters) => FunctionInfo.MethodInfo.InvokeAsync(Instance, parameters);

        public Task<TReturn> InjectAsync<TReturn>(params object[] parameters) => FunctionInfo.MethodInfo.InjectAsync<TReturn>(Instance, parameters);

        public Task InjectAsync(params object[] parameters) => FunctionInfo.MethodInfo.InjectAsync(Instance, parameters);
    }
}
