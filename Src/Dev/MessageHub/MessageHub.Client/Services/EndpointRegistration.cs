﻿using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace MessageHub.Client
{
    public class EndpointRegistration
    {
        private readonly ConcurrentDictionary<string, ResourceEndpointRegistration> _registry = new ConcurrentDictionary<string, ResourceEndpointRegistration>(StringComparer.OrdinalIgnoreCase);

        public EndpointRegistration()
        {
        }

        public EndpointRegistration Add(IEnumerable<ResourceEndpointRegistration> resourceEndpointRegistrations)
        {
            resourceEndpointRegistrations.Verify(nameof(resourceEndpointRegistrations)).IsNotNull();

            foreach(var item in resourceEndpointRegistrations)
            {
                _registry[item.Key] = item;
            }

            return this;
        }

        public bool TryGetValue(string resourceUri, out ResourceEndpointRegistration resourceEndpointRegistration)
        {
            resourceUri.Verify(nameof(resourceUri)).IsNotEmpty();

            if (_registry.TryGetValue(ResourceEndpointRegistration.CreateKey(resourceUri), out resourceEndpointRegistration)) return true;

            return false;
        }
    }
}