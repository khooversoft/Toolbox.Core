using Khooversoft.MessageHub.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MessageHub.Client
{
    public interface INameServer
    {
        Task<NodeRegistrationModel> Lookup(string nodeId);
    }
}
