using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessageHub.Management
{
    public static class ConversionExtensions
    {
        public static NodeRegistrationModel ConvertTo(this RouteRegistrationRequest subject)
        {
            subject.Verify(nameof(subject)).IsNotNull();

            return new NodeRegistrationModel
            {
                NodeId = subject.NodeId,
                Roles = subject?.Roles?.ToList(),
            };
        }

        public static RouteRegistrationRequest ConvertTo(this NodeRegistrationModel subject)
        {
            subject.Verify(nameof(subject)).IsNotNull();

            return new RouteRegistrationRequest
            {
                NodeId = subject.NodeId,
                Roles = subject?.Roles?.ToList(),
            };
        }
    }
}
