using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Khooversoft.MessageNet.Client
{
    public static class NodeIdExtensions
    {
        public static string QueueName(this Uri subject)
        {
            subject.Verify(nameof(subject)).IsNotNull();

            return subject.Host.ToEnumerable()
                .Concat(subject.LocalPath.Split('/'))
                .Do(x => string.Join("/", x));
        }
    }
}
