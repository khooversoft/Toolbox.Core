using Khooversoft.Toolbox.Standard;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Toolbox.BlockDocument
{
    public static class BlockChainToJson
    {
        public static string ToJson(this BlockChain blockChain)
        {
            blockChain.Verify(nameof(blockChain)).IsNotNull();

            return JsonConvert.SerializeObject(blockChain);
        }

        //public static T ToBlockChain
    }
}
