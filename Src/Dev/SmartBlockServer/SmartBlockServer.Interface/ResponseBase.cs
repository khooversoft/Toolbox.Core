using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.SmartBlockServer.Interface
{
    public abstract class ResponseBase
    {
        public string BlockChainUri { get; set; }

        public bool Completed { get; set; }

        public string ErrorMessage { get; set; }
    }
}
