using System;
using System.Collections.Generic;
using System.Text;

namespace MessageHub.Management
{
    public class NodeRegistrationModel
    {
        public string? NodeId { get; set; }

        public IReadOnlyList<string>? Roles { get; set; }
    }
}
