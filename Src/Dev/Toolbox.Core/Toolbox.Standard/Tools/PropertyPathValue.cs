// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System.Reflection;

namespace Khooversoft.Toolbox.Standard
{
    public class PropertyPathValue
    {
        public PropertyPathValue(string path, object value, PropertyInfo propertyInfo)
        {
            Path = path;
            Value = value;
            PropertyInfo = propertyInfo;
        }

        public string Path { get; }

        public object Value { get; }

        public PropertyInfo PropertyInfo { get; }
    }
}
