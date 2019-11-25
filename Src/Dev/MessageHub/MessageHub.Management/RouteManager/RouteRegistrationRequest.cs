using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessageHub.Management
{
    public class RouteRegistrationRequest
    {
        public string? NodeId { get; set; }

        public IReadOnlyList<string>? Roles { get; set; }
    }
}
