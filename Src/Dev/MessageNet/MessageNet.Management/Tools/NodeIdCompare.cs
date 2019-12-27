using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.MessageNet.Management
{
    public class NodeIdCompare
    {
        private readonly string _search;

        public NodeIdCompare(string search)
        {
            search.Verify(nameof(search)).IsNotNull();

            _search = search;
        }

        public bool Test(string nodeId)
        {
            if (_search == "*") return true;

            return _search.Equals(nodeId, StringComparison.OrdinalIgnoreCase);
        }
    }
}
