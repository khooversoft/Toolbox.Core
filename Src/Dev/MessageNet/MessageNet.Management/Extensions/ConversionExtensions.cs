// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.MessageHub.Interface;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Khooversoft.MessageHub.Management
{
    public static class ConversionExtensions
    {
        public static NodeRegistrationModel ConvertTo(this RouteRegistrationRequest subject, Uri uri)
        {
            subject.Verify(nameof(subject)).IsNotNull();

            return new NodeRegistrationModel(subject.NodeId!, uri.ToString());
        }

        public static RouteRegistrationRequest ConvertTo(this NodeRegistrationModel subject)
        {
            subject.Verify(nameof(subject)).IsNotNull();

            return new RouteRegistrationRequest { NodeId = subject.NodeId };
        }
    }
}
