using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Standard
{
    public interface IFunction
    {
        public object Instance { get; }

        public FunctionInfo FunctionInfo { get; }

        Task<TReturn> Invoke<TReturn>(params object[] parameters);

        Task Invoke(params object[] parameters);

        Task<TReturn> InjectAsync<TReturn>(params object[] parameters);

        Task InjectAsync(params object[] parameters);
    }
}
