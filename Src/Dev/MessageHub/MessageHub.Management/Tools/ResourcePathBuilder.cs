using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessageHub.Management
{
    public enum ResourceScheme
    {
        None,
        Queue,
        Topic,
        Event
    }

    public class ResourcePathBuilder
    {
        public ResourcePathBuilder()
        {
        }

        public ResourcePathBuilder(ResourceScheme scheme, string serviceBusName, string entityName)
        {
            scheme.Verify(nameof(scheme)).IsNotNull();
            scheme.Verify(nameof(scheme)).Assert(x => x != ResourceScheme.None, "Scheme is invalid");
            serviceBusName.Verify(nameof(serviceBusName)).IsNotEmpty();
            entityName.Verify(nameof(entityName)).IsNotEmpty();

            Scheme = scheme;
            ServiceBusName = serviceBusName;
            EntityName = entityName;
        }

        public ResourceScheme Scheme { get; set; } = ResourceScheme.None;

        public string? ServiceBusName { get; set; }

        public string? EntityName { get; set; }

        public ResourcePathBuilder SetScheme(ResourceScheme resourceScheme)
        {
            Scheme = resourceScheme;
            return this;
        }

        public ResourcePathBuilder SetServiceBusName(string serviceBusName)
        {
            serviceBusName.Verify(nameof(serviceBusName)).IsNotEmpty();

            ServiceBusName = serviceBusName;
            return this;
        }

        public ResourcePathBuilder SetEntityName(string entityName)
        {
            entityName.Verify(nameof(entityName)).IsNotEmpty();

            EntityName = entityName;
            return this;
        }

        public Uri Build()
        {
            Scheme.Verify(nameof(Scheme)).Assert(x => x != ResourceScheme.None, "Scheme is required");
            ServiceBusName!.Verify(nameof(ServiceBusName)).IsNotEmpty();
            EntityName!.Verify(nameof(EntityName)).IsNotEmpty();

            var builder = new UriBuilder();
            builder.Scheme = Scheme.ToString();

            var pathItems = new string[]
            {
                ServiceBusName!,
                EntityName!,
            };

            builder.Path = string.Join("/", pathItems);

            return builder.Uri;
        }
    }
}
