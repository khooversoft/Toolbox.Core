using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.MessageBroker
{
    public interface ITopicClient
    {
        public string Topic { get; }

        public void Post(byte[] message);

        public Task SendAsync(byte[] message);
    }
}
