using System;
using System.Collections.Generic;
using System.Text;

namespace Toolbox.Standard
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Do action on any object, used for fluent patterns, acts like a F# function in a pipe
        /// </summary>
        /// <typeparam name="T">object to work on</typeparam>
        /// <param name="self">instance of object</param>
        /// <param name="func">function to perform work</param>
        /// <returns></returns>
        public static TResult? Do<T, TResult>(this T? self, Func<T, TResult> func)
            where T : class
            where TResult : class
        {
            if (self == null)
            {
                return default;
            }

            return func(self);
        }
    }
}
