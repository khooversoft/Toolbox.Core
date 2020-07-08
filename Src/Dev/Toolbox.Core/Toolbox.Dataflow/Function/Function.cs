using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace KHooversoft.Toolbox.Dataflow
{
    public static class Function
    {
        /// <summary>
        /// Find all functions in exported types for assembly
        /// </summary>
        /// <typeparam name="TAttr">attribute to search for</typeparam>
        /// <param name="assembly">assembly to get exported types</param>
        /// <returns>list of function info</returns>
        public static IReadOnlyList<FunctionInfo> FindMethodsByAttribute<TAttr>(this Assembly assembly) where TAttr : Attribute
        {
            return assembly.VerifyNotNull(nameof(assembly))
                .ExportedTypes.FindMethodsByAttribute<TAttr>();
        }

        /// <summary>
        /// Find all functions in types
        /// </summary>
        /// <typeparam name="TAttr">attribute to search for</typeparam>
        /// <param name="types">types to search for</param>
        /// <returns>list of function info</returns>
        public static IReadOnlyList<FunctionInfo> FindMethodsByAttribute<TAttr>(this IEnumerable<Type> types) where TAttr : Attribute
        {
            return types
                .VerifyNotNull(nameof(types))
                .SelectMany(x => x.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                .Select(x => (methodInfo: x, attrs: x.GetCustomAttributes<TAttr>()))
                .Where(x => x.attrs.Count() > 0)
                .SelectMany(x => x.attrs, (o, i) => new FunctionInfo(o.methodInfo, i))
                .ToList();
        }
    }
}
