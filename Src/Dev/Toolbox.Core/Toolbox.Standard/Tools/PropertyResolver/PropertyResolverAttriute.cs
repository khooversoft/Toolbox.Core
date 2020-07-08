// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Toolbox.Standard
{
    /// <summary>
    /// Marks property on class to be used as property sources.
    /// Name will override property name if specified.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class PropertyResolverAttribute : Attribute
    {
        // This is a positional argument
        public PropertyResolverAttribute()
        {
            PropertyName = null;
        }

        public PropertyResolverAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }

        public string? PropertyName { get; }
    }
}
