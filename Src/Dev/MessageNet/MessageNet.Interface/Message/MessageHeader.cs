// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.MessageNet.Interface
{
    public class MessageHeader : INetMessageItem
    {
        public MessageHeader(MessageHeader subject)
        {
            MessageId = subject.MessageId;
            ToUri = subject.ToUri;
            FromUri = subject.FromUri;
            Method = subject.Method;
        }

        public MessageHeader(string toUri, string fromUri, string method)
        {
            toUri.Verify(nameof(toUri)).IsNotEmpty();
            fromUri.Verify(nameof(fromUri)).IsNotEmpty();
            method.Verify(nameof(method)).IsNotEmpty();

            ToUri = toUri.ToMessageUri().ToString();
            FromUri = fromUri.ToMessageUri().ToString();
            Method = method;
        }

        public MessageHeader(Guid messageId, string toUri, string fromUri, string method)
            : this(toUri, fromUri, method)
        {
            MessageId = messageId;
        }

        public MessageHeader(MessageUri toUri, MessageUri fromUri, string method)
        {
            ToUri = toUri.Verify(nameof(toUri)).IsNotNull().Value.ToString();
            FromUri = fromUri.Verify(nameof(fromUri)).IsNotNull().Value.ToString();
            Method = method.Verify(nameof(method)).IsNotEmpty().Value.ToString();
        }

        public Guid MessageId { get; } = Guid.NewGuid();

        public string ToUri { get; }

        public string FromUri { get; }

        public string Method { get; }

        public override bool Equals(object obj)
        {
            if (obj is MessageHeader header)
            {
                return MessageId == header.MessageId &&
                    ToUri == header.ToUri &&
                    FromUri == header.FromUri &&
                    Method == header.Method;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(MessageId, ToUri, FromUri, Method);
        }

        public static bool operator ==(MessageHeader v1, MessageHeader v2) => v1?.Equals(v2) ?? false;

        public static bool operator !=(MessageHeader v1, MessageHeader v2) => !v1?.Equals(v2) ?? false;
    }
}
