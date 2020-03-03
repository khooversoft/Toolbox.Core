using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.MessageNet.Interface
{
    public class NetMessageModel
    {
        public string? Version { get; set; }

        public IList<MessageHeaderModel>? Headers { get; set; }

        public IList<MessageActivityModel>? Activity { get; set; }

        public IList<MessageContentModel>? Content { get; set; }
    }
}
