using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace KHooversoft.Toolbox.Dataflow
{
    public interface IFunction
    {
        public object Instance { get; }

        public FunctionInfo FunctionInfo { get; }

        Task<T> Invoke<T>(params object[] parameters);

        Task Invoke(params object[] parameters);

        Task<T> InjectAsync<T>(params object[] parameters);

        Task InjectAsync(params object[] parameters);
    }
}
