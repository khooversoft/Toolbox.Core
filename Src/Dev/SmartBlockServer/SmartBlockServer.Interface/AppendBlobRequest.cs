﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.SmartBlockServer.Interface
{
    public class AppendBlobRequest
    {
        public string BlockChainUri { get; set; }

        public string Name { get; set; }

        public string ContentType { get; set; }

        public string Author { get; set; }

        public IReadOnlyList<byte> Content { get; set; }

        public string JwtIssuerSigner { get; set; }
    }

    public class AppendBlobResponse : ResponseBase
    {
        public string Name { get; set; }
    }
}
