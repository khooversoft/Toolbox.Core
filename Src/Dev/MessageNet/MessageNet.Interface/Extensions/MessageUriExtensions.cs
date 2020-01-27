using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.MessageNet.Interface
{
    public static class MessageUriExtensions
    {
        public static MessageUri ToMessageUri(this string subject) => MessageUriBuilder.Parse(subject).Build();
    }
}
