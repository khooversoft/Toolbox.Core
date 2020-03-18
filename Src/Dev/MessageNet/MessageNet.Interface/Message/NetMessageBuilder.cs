// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Khooversoft.MessageNet.Interface
{
    /// <summary>
    /// Build net message from message item such as header, activity, content.
    /// </summary>
    public class NetMessageBuilder : IEnumerable<INetMessageItem>
    {
        private readonly IReadOnlyList<INetMessageItem>? _fromMessage;

        public NetMessageBuilder() { }

        public NetMessageBuilder(NetMessage netMessage) => _fromMessage = netMessage.MessageItems.ToList();

        public List<INetMessageItem> MessageItems = new List<INetMessageItem>();

        /// <summary>
        /// Add message part
        /// </summary>
        /// <param name="messageItems">message items</param>
        /// <returns>this</returns>
        public NetMessageBuilder Add(params INetMessageItem[] messageItems)
        {
            MessageItems.AddRange(messageItems);
            return this;
        }

        public IEnumerator<INetMessageItem> GetEnumerator() => MessageItems.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => MessageItems.GetEnumerator();

        public NetMessage Build() => new NetMessage(MessageItems.Concat(_fromMessage ?? Enumerable.Empty<INetMessageItem>()));
    }
}
