using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.MessageNet.Client
{
    public static class ConvertionExtensions
    {
        public static NodeRegistrationModel ConvertTo(this RouteLookupResponse self)
        {
            self.Verify(nameof(self)).IsNotNull();

            return new NodeRegistrationModel(self.NodeId!, self.InputUri!);
        }
    }
}
