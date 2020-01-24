using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.SmartBlockServer.Interface
{
    public class AppendTextRequest
    {
        public string? BlockChainUri { get; set; }

        public string? Name { get; set; }

        public string? ContentType { get; set; }

        public string? Author { get; set; }

        public string? Content { get; set; }

        public string? JwtIssuerSigner { get; set; }
    }

    public class AppendTextResponse : ResponseBase
    {
        public string? Name { get; set; }
    }
}
