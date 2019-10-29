using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolbox.Core.Extensions.Configuration
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class OptionAttribute : Attribute
    {
        public OptionAttribute(params string[] helpText)
        {
            HelpText = helpText;
        }

        public string? Name { get; set; }

        public string[]? ShortCuts { get; set; }

        public string? Syntax { get; set; }

        public string[] HelpText { get; set; }

        public override string ToString()
        {
            return $"Name={Name}, Syntax={Syntax},  HelpText={string.Join(", ", HelpText ?? Enumerable.Empty<string>())}";
        }
    }
}
