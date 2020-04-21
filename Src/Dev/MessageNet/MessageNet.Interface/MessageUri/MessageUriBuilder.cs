// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Khooversoft.MessageNet.Interface
{
    public class MessageUriBuilder
    {
        private static StringTokenizer _tokenizer = new StringTokenizer().Add("/", ":");

        public MessageUriBuilder() { }

        /// <summary>
        /// Protocol, default to "MS"
        /// </summary>
        public string? Protocol { get; set; }

        /// <summary>
        /// Name space
        /// </summary>
        public string? Namespace { get; set; }

        /// <summary>
        /// Network ID
        /// </summary>
        public string? NetworkId { get; set; }

        /// <summary>
        /// Network ID
        /// </summary>
        public string? NodeId { get; set; }

        /// <summary>
        /// Route items
        /// </summary>
        public StringVectorBuilder Route { get; private set; } = new StringVectorBuilder();

        /// <summary>
        /// Set protocol, default is "MS"
        /// </summary>
        /// <param name="protocol">protocol to use</param>
        /// <returns>this</returns>
        public MessageUriBuilder SetProtocol(string protocol)
        {
            Protocol = protocol;
            return this;
        }

        /// <summary>
        /// Set network ID
        /// </summary>
        /// <param name="nameSpace">network id</param>
        /// <returns>this</returns>
        public MessageUriBuilder SetNamespace(string nameSpace)
        {
            Namespace = nameSpace;
            return this;
        }

        /// <summary>
        /// Set network ID
        /// </summary>
        /// <param name="networkId">network id</param>
        /// <returns>this</returns>
        public MessageUriBuilder SetNetworkId(string networkId)
        {
            NetworkId = networkId;
            return this;
        }

        /// <summary>
        /// Set network ID
        /// </summary>
        /// <param name="nodeId">network id</param>
        /// <returns>this</returns>
        public MessageUriBuilder SetNodeId(string nodeId)
        {
            NodeId = nodeId;
            return this;
        }

        /// <summary>
        /// Set route, null to clear
        /// </summary>
        /// <param name="route">route to set, empty string or null will clear</param>
        /// <returns>this</returns>
        public MessageUriBuilder SetRoute(string? route)
        {
            Route = new StringVectorBuilder().Parse(route ?? string.Empty);
            return this;
        }

        /// <summary>
        /// Parse Message URI and setup builder
        /// </summary>
        /// <param name="uri">URI string</param>
        /// <returns>message URI builder</returns>
        public static MessageUriBuilder Parse(string uri)
        {
            string syntaxError = $"Syntax error in {uri}";

            IReadOnlyList<IToken> tokens = _tokenizer.Parse(uri);
            var stack = new Stack<string>(tokens.Select(x => x.Value).Reverse());

            var builder = new MessageUriBuilder();

            var instructionStack = new List<Action<string>>
            {
                x => {
                    switch(stack.TryPeek(out string peekToken) == true && peekToken == ":")
                    {
                        case true:
                            builder.Protocol = x;

                            string token;

                            stack.TryPop(out token).VerifyAssert(x => x == true && token == ":", syntaxError);
                            stack.TryPop(out token).VerifyAssert(x => x == true && token == "/", syntaxError);
                            stack.TryPop(out token).VerifyAssert(x => x == true && token == "/", syntaxError);
                            break;

                        default:
                            stack.Push(x);
                            break;

                    }
                },
                x => builder.Namespace = x,
                x => x.VerifyAssert(x => x == "/", syntaxError),
                x => builder.NetworkId = x,
                x => x.VerifyAssert(x => x == "/", syntaxError),
                x => builder.NodeId = x,
                x => builder.Route = new StringVectorBuilder().Add(x.ToEnumerable().Concat(stack.Drain()).ToArray()),
            }
            .Reverse<Action<string>>()
            .ToStack();

            while (stack.Count > 0 && instructionStack.Count > 0)
            {
                instructionStack.Pop()(stack.Pop());
            }

            (stack.Count == 0 && instructionStack.Count <= 1).VerifyAssert(x => x == true, syntaxError);

            return builder;
        }

        /// <summary>
        /// Build message URI
        /// </summary>
        /// <returns>message URI</returns>
        public MessageUri Build()
        {
            return new MessageUri(Protocol, Namespace, NetworkId!, NodeId!, Route?.Build()?.ToString());
        }
    }
}
