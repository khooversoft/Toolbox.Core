using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.MessageNet.Interface
{
    public class MessageHeaderModel
    {
        public Guid MessageId { get; set; }

        public string? ToUri { get; set; }

        public string? FromUri { get; set; }

        public string? Method { get; set; }

        public IDictionary<string, string>? Properties { get; set; }
    }
}
