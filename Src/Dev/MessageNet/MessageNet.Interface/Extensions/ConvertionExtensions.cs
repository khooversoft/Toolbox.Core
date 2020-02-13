// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.MessageNet.Interface
{
    public static class ConvertionExtensions
    {
        public static NodeRegistrationModel ConvertTo(this RouteLookupResponse self)
        {
            self.Verify(nameof(self)).IsNotNull();

            return new NodeRegistrationModel(self.NetworkId!, self.NodeId!, self.InputUri!);
        }

        public static NodeRegistrationModel ConvertTo(this RouteRegistrationResponse subject)
        {
            subject.Verify(nameof(subject)).IsNotNull();

            return new NodeRegistrationModel(subject.NetworkId!, subject.NodeId!, subject.InputQueueUri!);
        }
    }
}
