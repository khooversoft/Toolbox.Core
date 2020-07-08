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
    public static class ReflectionTools
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

        /// <summary>
        /// Invoke method async on class async.  Parameters must be in order and of required type.
        /// 
        /// Method must return Task of T
        /// </summary>
        /// <typeparam name="T">return type</typeparam>
        /// <param name="methodInfo">method information</param>
        /// <param name="instance">object instance</param>
        /// <param name="parameters">required parameters</param>
        /// <returns>return value of method</returns>
        public static Task<T> InvokeAsync<T>(this MethodInfo methodInfo, object instance, params object[] parameters) => (Task<T>)methodInfo.Invoke(instance, parameters);

        /// <summary>
        /// Invoke method async on class async.  Parameters must be in order and of required type. 
        /// </summary>
        /// <param name="methodInfo">method information</param>
        /// <param name="instance">instance of class</param>
        /// <param name="parameters">required parameters</param>
        /// <returns>task</returns>
        public static Task InvokeAsync(this MethodInfo methodInfo, object instance, params object[] parameters) => (Task)methodInfo.Invoke(instance, parameters);

        /// <summary>
        /// Inject parameters on async method.  Parameters can be in any order but must match (assignable) to method's parameters.
        /// </summary>
        /// <typeparam name="T">return type</typeparam>
        /// <param name="methodInfo">method information</param>
        /// <param name="instance">class instance</param>
        /// <param name="parameters">required parameters</param>
        /// <returns>return value of method</returns>
        public static Task<T> InjectAsync<T>(this MethodInfo methodInfo, object instance, params object[] parameters)
        {
            instance.VerifyNotNull(nameof(instance));

            var parameterTypes = BuildParameters(methodInfo, parameters);
            return methodInfo.InvokeAsync<T>(instance, parameterTypes);
        }

        /// <summary>
        /// Inject parameters on async method.  Parameters can be in any order but must match (assignable) to method's parameters.
        /// </summary>
        /// <param name="methodInfo">method information</param>
        /// <param name="instance">class instance</param>
        /// <param name="parameters">required parameters</param>
        /// <returns>task</returns>
        public static Task InjectAsync(this MethodInfo methodInfo, object instance, params object[] parameters)
        {
            instance.VerifyNotNull(nameof(instance));

            var parameterTypes = BuildParameters(methodInfo, parameters);
            return methodInfo.InvokeAsync(instance, parameterTypes);
        }

        /// <summary>
        /// Order parameters provided to the order a method parameters require.
        /// Will match based on type or take the default if provided
        /// </summary>
        /// <param name="methodInfo">method information</param>
        /// <param name="parameters">required parameters</param>
        /// <returns>array of order parameters</returns>
        public static object[] BuildParameters(this MethodInfo methodInfo, object[] parameters)
        {
            methodInfo.VerifyNotNull(nameof(methodInfo));
            parameters.VerifyNotNull(nameof(parameters));

            return methodInfo
                .GetParameters()
                .Select((x, i) => parameters.FirstOrDefault(y => x.ParameterType.IsAssignableFrom(y.GetType())) ?? methodInfo.GetParameters()[i].DefaultValue)
                .ToArray();
        }

        /// <summary>
        /// Return missing parameter for method base on filtered types
        /// </summary>
        /// <param name="methodInfo">methodInfo of the method</param>
        /// <param name="filterOut">types to filter out</param>
        /// <returns></returns>
        public static Type[] GetMissingParameters(this MethodInfo methodInfo, Type[] filterOut)
        {
            methodInfo.VerifyNotNull(nameof(methodInfo));
            filterOut.VerifyNotNull(nameof(filterOut));

            return methodInfo
                .GetParameters()
                .Where((x, i) => (filterOut.FirstOrDefault(filterType => x.ParameterType.IsAssignableFrom(filterType)) == null && methodInfo.GetParameters()[i].DefaultValue == DBNull.Value))
                .Select(x => x.ParameterType)
                .ToArray();
        }
    }
}
