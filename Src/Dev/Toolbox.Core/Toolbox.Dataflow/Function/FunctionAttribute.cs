using System;
using System.Collections.Generic;
using System.Text;

namespace KHooversoft.Toolbox.Dataflow
{
    /// <summary>
    /// Function attribute used to defined a function
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class FunctionAttribute : Attribute
    {
        public FunctionAttribute() { }

        public FunctionAttribute(string name)
        {
            Name = name;
        }

        public string? Name { get; }
    }
}
