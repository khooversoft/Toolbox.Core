using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.SmartBlockServer.Interface
{
    public class ReadBlockChainRequest
    {
        public string? BlockChainUri { get; set; }

        public int StartIndex { get; set; }

        public int EndIndex { get; set; }
    }

    public class ReadBlockChainResponse : ResponseBase
    {
        public IReadOnlyList<string>? Blocks { get; set; }
    }
}
