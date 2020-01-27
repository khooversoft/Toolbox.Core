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
        private const string _regexPattern = @"^[a-zA-Z0-9_\-\.]*$";
        private const string _regexRoutePattern = @"^[a-zA-Z0-9_\-\./]*$";

        private readonly Regex _regex = new Regex(_regexPattern);
        private readonly Regex _regexRoute = new Regex(_regexRoutePattern);

        public MessageUri(string protocol, string networkId, string nodeId, string? route = null)
        {
            Func<string, string, string> verifyValid = (x, name) => x.Verify(name)
                .IsNotEmpty()
                .Assert(x => _regex.Match(x).Success, $"{name} is not valid, regex={_regexPattern}")
                .Value;

            Protocol = verifyValid(protocol, nameof(protocol));
            NetworkId = verifyValid(networkId, nameof(networkId));
            NodeId = verifyValid(nodeId, nameof(nodeId));

            Route = route
                ?.Split("/", StringSplitOptions.RemoveEmptyEntries)
                ?.Select(x => x.Verify("route").Assert(x => _regexRoute.Match(x).Success, $"route is not value, regex={_regexRoutePattern}").Value)
                ?.Do(x => string.Join("/", x));
        }

        /// <summary>
        /// Protocol, default to "MS"
        /// </summary>
        public string Protocol { get; }

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
            return $"{Protocol}://{NetworkId}/{NodeId}"
                .Do(x => Route.IsEmpty() ? x : $"{x}/{Route}");
        }

        /// <summary>
        /// Get queue name (network ID + Node ID)
        /// </summary>
        /// <returns></returns>
        public string GetQueueName()
        {
            return NetworkId + "/" + NodeId;
        }
    }
}
