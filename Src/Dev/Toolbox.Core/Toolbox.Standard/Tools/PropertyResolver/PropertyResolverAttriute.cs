using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Toolbox.Standard
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class PropertyResolverAttribute : Attribute
    {
        // This is a positional argument
        public PropertyResolverAttribute()
        {
        }
    }
}
