// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Toolbox.Extensions.Configuration
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class OptionHelpAttribute : Attribute
    {
        public OptionHelpAttribute(HelpArea area, string helpText)
        {
            Area = area;
            HelpText = helpText;
        }

        /// <summary>
        /// Help area
        /// </summary>
        public HelpArea Area { get; }

        /// <summary>
        /// Help text
        /// </summary>
        public string HelpText { get; }

        /// <summary>
        /// If help is about a command, this is the left column
        /// </summary>
        public string? Command { get; set; }

        public override string ToString()
        {
            return $"Area={Area}, Command={Command}, HelpText ={HelpText}";
        }
    }
}
