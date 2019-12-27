using Khooversoft.MessageNet.Client;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartBlockServer
{
    internal class RegisterRoute : IAction
    {
        private readonly IOption _option;
        private readonly MessageNetClient _messageNetClient;

        public RegisterRoute(IOption option, MessageNetClient messageNetClient)
        {
            _option = option;
            _messageNetClient = messageNetClient;
        }

        public Task Run(IWorkContext context)
        {
            throw new NotImplementedException();
        }
    }
}
