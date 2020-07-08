using Khooversoft.Toolbox.Standard;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KHooversoft.Toolbox.Dataflow
{
    /// <summary>
    /// Function host to manage a set of functions
    /// </summary>
    public class FunctionHost : IDisposable
    {
        private readonly IReadOnlyList<FunctionInfo> _functionInfos;
        private readonly ConcurrentDictionary<string, IFunction> _functionLookup = new ConcurrentDictionary<string, IFunction>(StringComparer.OrdinalIgnoreCase);
        private IReadOnlyList<DeclaringTypeInstance> _instances;

        public FunctionHost(IEnumerable<FunctionInfo> functionInfos, Func<Type, object>? createFactory = null)
        {
            functionInfos.VerifyNotNull(nameof(functionInfos)).VerifyAssert(x => x.Count() > 0, $"{nameof(functionInfos)} is empty");

            _functionInfos = functionInfos.ToList();

            _instances = _functionInfos
                .GroupBy(x => x.MethodInfo.DeclaringType.FullName, (k, funcs) => new DeclaringTypeInstance(construct(funcs.First().GetDeclaringType()), funcs.ToArray()))
                .ToList();

            GetFunctions()
                .ForEach(x => _functionLookup.TryAdd(x.FunctionInfo.Name, x).VerifyAssert(y => y, $"Duplicate function {x.FunctionInfo.Name}"));

            object construct(Type type) => createFactory switch
            {
                Func<Type, object> factory => factory(type),
                _ => Activator.CreateInstance(type)
            };
        }

        public IFunction this[string name] => _functionLookup[name.VerifyNotEmpty(name)];

        public int Count => _functionLookup.Count;

        public IReadOnlyList<IFunction> GetFunctions() => (_instances ?? Array.Empty<DeclaringTypeInstance>())
            .SelectMany(x => x.GetFunctions())
            .ToList();

        public bool TryGetFunction(string name, out IFunction function) => _functionLookup.TryGetValue(name, out function);

        public void Dispose()
        {
            var instances = Interlocked.Exchange(ref _instances, null!);
            instances?.ForEach(x => x.Dispose());
        }
    }
}
