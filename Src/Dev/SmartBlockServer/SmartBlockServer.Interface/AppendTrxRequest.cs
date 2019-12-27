using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.SmartBlockServer.Interface
{
    public class AppendTrxRequest
    {
        public string BlockChainUri { get; set; }

        public string ReferenceId { get; set; }

        public string TransactionType { get; set; }

        public long Md4Value { get; set; }

        public string JwtIssuerSigner { get; set; }
    }

    public class AppendTrxResponse : ResponseBase
    {
        public string ReferenceId { get; set; }
    }
}
