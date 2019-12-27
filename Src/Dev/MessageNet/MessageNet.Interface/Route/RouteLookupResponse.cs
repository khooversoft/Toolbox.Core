using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.MessageNet.Interface
{
    public class RouteLookupResponse
    {
        public string? NodeId { get; set; }

        public string? InputUri { get; set; }

        public IReadOnlyDictionary<string, string>? UriRoutes { get; set; }
    }
}
