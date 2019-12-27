using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.SmartBlockServer.Interface
{
    public class CreateBlockChainRequest
    {
        public string BlockChainUri { get; set; }

        public string Tenant { get; set; }

        public string ContractId { get; set; }
    }

    public class CreateBlockChainResponse : ResponseBase
    {
        public string BlockChainUri { get; set; }
    }
}
