// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Toolbox.Configuration
{
    public class OptionHelp
    {
        public OptionHelp(HelpArea area, string text)
        {
            Area = area;
            Text = new string[] { text };
        }

        public OptionHelp(HelpArea option, string command, string[] text)
        {
            Area = option;
            Command = command;
            Text = text;
        }

        public HelpArea Area { get; }

        public string? Command { get; }

        public string[] Text { get; }
    }
}
