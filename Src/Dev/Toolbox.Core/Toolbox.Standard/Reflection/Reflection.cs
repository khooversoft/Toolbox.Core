using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Standard
{
    public static class Reflection
    {
        /// <summary>
        /// Load assembly into default context
        /// </summary>
        /// <param name="assemblyPath">path to assembly</param>
        /// <returns>assembly</returns>
        public static Assembly LoadFromAssemblyPath(string assemblyPath)
        {
            assemblyPath.VerifyNotEmpty(nameof(assemblyPath)).VerifyAssert(x => File.Exists(assemblyPath), x => $"Assembly {x} does not exist");
            return AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath);
        }

        public static Task<TReturn> InvokeAsync<TReturn>(this MethodInfo methodInfo, object instance, params object[] parameters) => (Task<TReturn>)methodInfo.Invoke(instance, parameters);

        public static Task InvokeAsync(this MethodInfo methodInfo, object instance, params object[] parameters) => (Task)methodInfo.Invoke(instance, parameters);

        public static Task<TReturn> InjectAsync<TReturn>(this MethodInfo methodInfo, object instance, params object[] parameters)
        {
            instance.VerifyNotNull(nameof(instance));

            var parameterTypes = BuildParameters(methodInfo, parameters);
            return methodInfo.InvokeAsync<TReturn>(instance, parameterTypes);
        }

        public static Task InjectAsync(this MethodInfo methodInfo, object instance, params object[] parameters)
        {
            instance.VerifyNotNull(nameof(instance));

            var parameterTypes = BuildParameters(methodInfo, parameters);
            return methodInfo.InvokeAsync(instance, parameterTypes);
        }

        public static object[] BuildParameters(this MethodInfo methodInfo, object[] parameters)
        {
            methodInfo.VerifyNotNull(nameof(methodInfo));
            parameters.VerifyNotNull(nameof(parameters));

            return methodInfo
                .GetParameters()
                .Select((x, i) => parameters.FirstOrDefault(y => x.ParameterType.IsAssignableFrom(y.GetType())) ?? methodInfo.GetParameters()[i].DefaultValue)
                .ToArray();
        }

        public static Type[] GetMissingParameters(this MethodInfo methodInfo, object[] filterOut)
        {
            methodInfo.VerifyNotNull(nameof(methodInfo));
            filterOut.VerifyNotNull(nameof(filterOut));

            return methodInfo
                .GetParameters()
                .Where((x, i) => (filterOut.FirstOrDefault(y => x.ParameterType.IsAssignableFrom(y.GetType())) == null && methodInfo.GetParameters()[i].DefaultValue == DBNull.Value))
                .Select(x => x.ParameterType)
                .ToArray();
        }
    }
}
