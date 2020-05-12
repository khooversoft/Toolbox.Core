// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.MessageNet.Interface
{
    public class MessageContent : INetMessageItem
    {
        public MessageContent(string contentType, string content)
        {
            contentType.VerifyNotEmpty(nameof(contentType));
            content.VerifyNotEmpty(nameof(content));

            ContentType = contentType;
            Content = content;
        }

        public Guid ContentId { get; } = Guid.NewGuid();

        public string ContentType { get; }

        public string Content { get; }

        public override bool Equals(object? obj)
        {
            return obj is MessageContent content &&
                   ContentId.Equals(content.ContentId) &&
                   ContentType == content.ContentType &&
                   Content == content.Content;
        }

        public override int GetHashCode() => HashCode.Combine(ContentId, ContentType, Content);

        public static bool operator ==(MessageContent? left, MessageContent? right) => EqualityComparer<MessageContent>.Default.Equals(left!, right!);

        public static bool operator !=(MessageContent? left, MessageContent? right) => !(left == right);

        public static MessageContent Create<T>(T data) where T : class => new MessageContent(typeof(T).Name, data switch { string value => value, _ => JsonConvert.SerializeObject(data) });
    }
}
