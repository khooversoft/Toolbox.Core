// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;

namespace SmartBlockServer
{
    internal interface IOption
    {
        TelemetryType ConsoleLevel { get; set; }
        bool Help { get; set; }
        string LoggingFolder { get; set; }
        string NameServerUri { get; set; }
        string NodeId { get; set; }
        bool Run { get; set; }
        string ServiceBusConnection { get; set; }
        bool UnRegister { get; }
    }
}