﻿// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.MessageNet.Interface
{
    /// <summary>
    /// Resource path builder for queue, topic, event
    /// </summary>
    public class ResourcePathBuilder
    {
        public ResourcePathBuilder()
        {
        }

        public ResourcePathBuilder(string uri)
        {
            uri.Verify(nameof(uri)).IsNotEmpty();

            var uriBuilder = new UriBuilder(uri);
            Scheme = (ResourceScheme)Enum.Parse(typeof(ResourceScheme), uriBuilder.Scheme);
            ServiceBusName = uriBuilder.Host;
            EntityName = new StringVector(uriBuilder.Path, "/");
        }

        public ResourcePathBuilder(ResourceScheme scheme, string serviceBusName, params string[] entityNames)
        {
            scheme.Verify(nameof(scheme)).IsNotNull();
            scheme.Verify(nameof(scheme)).Assert(x => x != ResourceScheme.None, "Scheme is invalid");
            serviceBusName.Verify(nameof(serviceBusName)).IsNotEmpty();
            entityNames.Verify(nameof(entityNames)).IsNotNull();

            Scheme = scheme;
            ServiceBusName = serviceBusName;
            EntityName = new StringVector(entityNames, "/", false);
        }

        public ResourceScheme Scheme { get; set; } = ResourceScheme.None;

        public string ServiceBusName { get; set; } = string.Empty;

        public StringVector EntityName { get; set; } = StringVector.Empty;

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

        public ResourcePathBuilder SetEntityName(params string[] entityNames)
        {
            entityNames.Verify(nameof(entityNames)).IsNotNull();

            EntityName = new StringVector(entityNames, "/", false);
            return this;
        }

        public Uri Build()
        {
            Scheme.Verify(nameof(Scheme)).Assert(x => x != ResourceScheme.None, "Scheme is required");
            ServiceBusName!.Verify(nameof(ServiceBusName)).IsNotEmpty();
            EntityName!.Verify(nameof(EntityName)).IsNotNull();

            var builder = new UriBuilder
            {
                Scheme = Scheme.ToString(),
                Host = ServiceBusName!,
                Path = EntityName!
            };

            return builder.Uri;
        }
    }
}
