using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace KHooversoft.Toolbox.Dataflow
{
    /// <summary>
    /// Declaring type instance for a class with n functions
    /// </summary>
    public class DeclaringTypeInstance : IDisposable
    {
        private object? _instance;
        private readonly IReadOnlyList<IFunction> _functions;

        public DeclaringTypeInstance(object instance, params FunctionInfo[] functionInfos)
        {
            instance.VerifyNotNull(nameof(instance));

            _instance = instance;
            FunctionInfos = functionInfos;

            _functions = functionInfos.Select(x => new FunctionInvoke(instance, x)).ToList();
        }

        public object? Instance => _instance;

        public IReadOnlyList<FunctionInfo> FunctionInfos { get; }

        public IReadOnlyList<IFunction> GetFunctions() => _functions;

        public void Dispose()
        {
            object? instance = Interlocked.Exchange(ref _instance, null!);

            switch (instance)
            {
                case IDisposable disposable:
                    disposable.Dispose();
                    break;
            }
        }
    }
}
