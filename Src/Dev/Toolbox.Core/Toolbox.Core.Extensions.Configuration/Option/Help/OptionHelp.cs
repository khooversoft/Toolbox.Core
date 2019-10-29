using System;
using System.Collections.Generic;
using System.Text;

namespace Toolbox.Core.Extensions.Configuration
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
