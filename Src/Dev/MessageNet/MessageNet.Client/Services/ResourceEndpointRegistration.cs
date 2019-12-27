using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.MessageNet.Client
{
    public class ResourceEndpointRegistration
    {
        public ResourceEndpointRegistration(ResourceScheme scheme, string serviceBusNamespace, string connectionString)
        {
            serviceBusNamespace.Verify(nameof(serviceBusNamespace)).IsNotEmpty();
            connectionString.Verify(nameof(connectionString)).IsNotEmpty();

            Scheme = scheme;
            ServiceBusNamespace = serviceBusNamespace;
            ConnectionString = connectionString;
        }

        public ResourceScheme Scheme { get; }

        public string ServiceBusNamespace { get; }

        public string ConnectionString { get; }

        public string Key => Scheme.ToString() + ":" + ServiceBusNamespace;

        public static string CreateKey(string resourceUri)
        {
            var resourcePath = new ResourcePathBuilder(resourceUri);
            return resourcePath.Scheme.ToString() + ":" + resourcePath.ServiceBusName;
        }
    }
}
