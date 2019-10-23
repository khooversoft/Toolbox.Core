using System;
using System.Collections.Generic;
using System.Text;

namespace Toolbox.Standard
{
    public static class StringExtensions
    {
        public static StringPath ParsePath(this string value, string delimiter = "/")
        {
            return new StringPathBuilder(delimiter).Parse(value).Build();
        }
    }
}
