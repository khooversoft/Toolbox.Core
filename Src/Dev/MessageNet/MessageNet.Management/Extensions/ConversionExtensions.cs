// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Khooversoft.MessageNet.Management
{
    public static class ConversionExtensions
    {
        public static NodeRegistrationModel ConvertTo(this RouteRegistrationRequest subject, string inputUri)
        {
            subject.Verify(nameof(subject)).IsNotNull();

            return new NodeRegistrationModel(subject.NetworkId!, subject.NodeId!, inputUri);
        }

        public static RouteRegistrationRequest ConvertTo(this NodeRegistrationModel subject)
        {
            subject.Verify(nameof(subject)).IsNotNull();

            return new RouteRegistrationRequest { NodeId = subject.NodeId };
        }
    }
}
