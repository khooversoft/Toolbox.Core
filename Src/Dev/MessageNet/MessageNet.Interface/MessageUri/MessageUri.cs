// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace Khooversoft.MessageNet.Interface
{
    public class MessageUri
    {
        private const string _protocolPatternText = @"^[a-zA-Z][a-zA-Z0-9]+$";

        private readonly Regex _pattern = new Regex(_protocolPatternText, RegexOptions.Compiled);

        public MessageUri(string nameSpace, string networkId, string nodeId)
            : this(null, nameSpace, networkId, nodeId)
        {
        }

        public MessageUri(string? protocol, string? nameSpace, string? networkId, string nodeId, string? route = null)
        {
            protocol ??= "msgnet";
            nameSpace ??= "default";
            networkId ??= "default";

            QueueId.Verify(nameSpace, networkId, nodeId);

            Protocol = protocol.Verify(nameof(protocol)).Assert(x => _pattern.Match(x).Success, $"Is not valid, regex={_protocolPatternText}").Value;
            Namespace = nameSpace;
            NetworkId = networkId;
            NodeId = nodeId;

            Route = route
                ?.Split("/", StringSplitOptions.RemoveEmptyEntries)
                ?.Select(x => x.Verify("route").Assert(x => _pattern.Match(x).Success, $"route is not value, regex={_protocolPatternText}").Value)
                ?.Do(x => string.Join("/", x));
        }

        /// <summary>
        /// Protocol, default to "MS"
        /// </summary>
        public string Protocol { get; }

        /// <summary>
        /// Message name space
        /// </summary>
        public string Namespace { get; }

        /// <summary>
        /// Network ID
        /// </summary>
        public string NetworkId { get; }

        /// <summary>
        /// Network ID
        /// </summary>
        public string NodeId { get; }

        /// <summary>
        /// Optional route path
        /// </summary>
        public string? Route { get; }

        /// <summary>
        /// Convert URI to string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Protocol}://{Namespace}/{NetworkId}/{NodeId}"
                .Do(x => Route.IsEmpty() ? x : $"{x}/{Route}");
        }

        public string ToString(string format)
        {
            var properties = new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>("protocol", Protocol),
                new KeyValuePair<string, string>("namespace", Namespace),
                new KeyValuePair<string, string>("networkId", NetworkId),
                new KeyValuePair<string, string>("nodeId", NodeId),
            };

            return new PropertyResolver(properties).Resolve(format);
        }
    }
}
