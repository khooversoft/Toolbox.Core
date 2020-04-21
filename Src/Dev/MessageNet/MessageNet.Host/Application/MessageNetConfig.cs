using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Khooversoft.MessageNet.Host
{
    public class MessageNetConfig : IMessageNetConfig
    {
        public MessageNetConfig(params NamespaceRegistration[] namespaceRegistrations)
        {
            namespaceRegistrations
                .VerifyNotNull(nameof(namespaceRegistrations))
                .VerifyAssert(x => x.Count() > 0, "Must have at least one registration");

            Registrations = namespaceRegistrations.ToDictionary(x => x.Namespace, x => x, StringComparer.OrdinalIgnoreCase);
        }

        public IReadOnlyDictionary<string, NamespaceRegistration> Registrations { get; }
    }
}
