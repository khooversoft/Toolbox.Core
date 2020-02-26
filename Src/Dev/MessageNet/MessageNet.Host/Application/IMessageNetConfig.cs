using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.MessageNet.Host
{
    public interface IMessageNetConfig
    {
        IReadOnlyDictionary<string, NamespaceRegistration> Registrations { get; }
    }
}
