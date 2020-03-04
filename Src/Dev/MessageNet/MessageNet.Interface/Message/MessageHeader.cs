// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Khooversoft.MessageNet.Interface
{
    public class MessageHeader : INetMessageItem
    {
        public MessageHeader(MessageHeader subject)
        {
            subject.Verify(nameof(subject)).IsNotNull();

            MessageId = subject.MessageId;
            ToUri = subject.ToUri;
            FromUri = subject.FromUri;
            Method = subject.Method;
            Properties = subject.Properties.ToDictionary(x => x.Key, x => x.Value, StringComparer.OrdinalIgnoreCase);
        }

        public MessageHeader(string toUri, string fromUri, string method, params KeyValuePair<string, string>[] properties)
        {
            toUri.Verify(nameof(toUri)).IsNotEmpty();
            fromUri.Verify(nameof(fromUri)).IsNotEmpty();
            method.Verify(nameof(method)).IsNotEmpty();

            ToUri = toUri.ToMessageUri().ToString();
            FromUri = fromUri.ToMessageUri().ToString();
            Method = method;
            Properties = properties.ToDictionary(x => x.Key, x => x.Value, StringComparer.OrdinalIgnoreCase);
        }

        public MessageHeader(Guid messageId, string toUri, string fromUri, string method, params KeyValuePair<string, string>[] properties)
            : this(toUri, fromUri, method, properties)
        {
            MessageId = messageId;
        }

        public MessageHeader(MessageUri toUri, MessageUri fromUri, string method, params KeyValuePair<string, string>[] properties)
        {
            ToUri = toUri.Verify(nameof(toUri)).IsNotNull().Value.ToString();
            FromUri = fromUri.Verify(nameof(fromUri)).IsNotNull().Value.ToString();
            Method = method.Verify(nameof(method)).IsNotEmpty().Value.ToString();
            Properties = properties.ToDictionary(x => x.Key, x => x.Value, StringComparer.OrdinalIgnoreCase);
        }

        public Guid MessageId { get; } = Guid.NewGuid();

        public string ToUri { get; }

        public string FromUri { get; }

        public string Method { get; }

        public IReadOnlyDictionary<string, string> Properties { get; }

        public override bool Equals(object obj)
        {
            if (obj is MessageHeader header)
            {
                return ToUri == header.ToUri &&
                    FromUri == header.FromUri &&
                    Method == header.Method &&
                    Properties.Count == header.Properties.Count &&

                    Properties.OrderBy(x => x.Key)
                        .Zip(header.Properties.OrderBy(x => x.Key), (o, i) => (o, i))
                        .All(x => x.o.Key == x.i.Key && x.o.Value == x.i.Value);
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
