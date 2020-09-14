// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Khooversoft.MessageNet.Interface
{
    /// <summary>
    /// Net message, only the header is required
    ///
    /// Net message is a container for n number of message items.  The primary purpose of managing multiple message items per message
    /// is to provide additional information for routing, auditing, and message fulfillment.
    /// 
    /// Only one message header is required.
    /// 
    /// Message precedent is first one encountered in the message item list.
    /// </summary>
    public class NetMessage
    {
        private MessageActivity? _activity;
        private MessageContent? _content;
        private IReadOnlyList<MessageHeader>? _messageHeaders;
        private IReadOnlyList<MessageActivity>? _messageActivities;
        private IReadOnlyList<MessageContent>? _messageContents;

        public NetMessage(IEnumerable<INetMessageItem> messageItems)
        {
            messageItems.VerifyNotNull(nameof(messageItems));

            Version = "1.0.0.0";
            MessageItems = messageItems.ToList();

            Header = MessageItems
                .OfType<MessageHeader>()
                .FirstOrDefault()
                .VerifyNotNull("MessageHeader is not in message items");
        }

        public NetMessage(string version, IEnumerable<INetMessageItem> messageItems)
        {
            version.VerifyNotEmpty(nameof(version));
            messageItems.VerifyNotNull(nameof(messageItems));

            Version = version;
            MessageItems = messageItems.ToList();

            Header = MessageItems
                .OfType<MessageHeader>()
                .FirstOrDefault()
                .VerifyNotNull("MessageHeader is not in message items");
        }

        /// <summary>
        /// Message items
        /// </summary>
        public IReadOnlyList<INetMessageItem> MessageItems { get; }

        /// <summary>
        /// Message version
        /// </summary>
        public string Version { get; }

        /// <summary>
        /// Current header
        /// </summary>
        public MessageHeader Header { get; }

        /// <summary>
        /// Current activity, can be null
        /// </summary>
        public MessageActivity? Activity => _activity ??= MessageItems.OfType<MessageActivity>().FirstOrDefault();

        /// <summary>
        /// Current content, can be null
        /// </summary>
        public MessageContent? Content => _content ??= MessageItems.OfType<MessageContent>().FirstOrDefault();

        /// <summary>
        /// Enumerate all headers, current one will be first
        /// </summary>
        public IReadOnlyList<MessageHeader> Headers => _messageHeaders ??= MessageItems.OfType<MessageHeader>().ToList();

        /// <summary>
        /// Enumerate all activities, current one will be first
        /// </summary>
        public IReadOnlyList<MessageActivity> Activities => _messageActivities ??= MessageItems.OfType<MessageActivity>().ToList();

        /// <summary>
        /// Enumerate all contents, current one will be first
        /// </summary>
        public IReadOnlyList<MessageContent> Contents => _messageContents ??= MessageItems.OfType<MessageContent>().ToList();

        public override bool Equals(object? obj)
        {
            return obj is NetMessage header &&
                MessageItems
                    .Zip(header.MessageItems, (v1, v2) => (v1, v2))
                    .All(x => x.v1.Equals(x.v2));
        }

        public override int GetHashCode()
        {
            HashCode hashCode = new HashCode();
            MessageItems.ForEach(x => hashCode.Add(x));
            return hashCode.ToHashCode();
        }

        public override string ToString()
        {
            return $"NetMessage: Id: {Header.MessageId}, ToUri: {Header.ToUri}, FromUri: {Header.FromUri}, Method: {Header.Method}, Items.Count={MessageItems.Count}";
        }

        public static bool operator ==(NetMessage? v1, NetMessage? v2) => v1?.Equals(v2) ?? false;

        public static bool operator !=(NetMessage? v1, NetMessage? v2) => !v1?.Equals(v2) ?? false;
    }
}
