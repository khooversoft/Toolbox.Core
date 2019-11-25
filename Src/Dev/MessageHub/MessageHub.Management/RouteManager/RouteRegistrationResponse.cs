using System;
using System.Collections.Generic;
using System.Text;

namespace MessageHub.Management
{
    public class RouteRegistrationResponse
    {
        public string? InputQueueUri { get; set; }

        public IReadOnlyList<RouteRoleModel>? Routes { get; set; }
    }
}
