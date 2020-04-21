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

        public override bool Equals(object obj)
        {
            return obj is MessageContent subject &&
                ContentType == subject.ContentType &&
                Content == subject.Content;
        }

        public override int GetHashCode() => HashCode.Combine(ContentType, Content);

        public static bool operator ==(MessageContent v1, MessageContent V2) => v1?.Equals(V2) == true;

        public static bool operator !=(MessageContent v1, MessageContent v2) => !(v1 == v2);

        public static MessageContent Create<T>(T data) where T : class => new MessageContent(typeof(T).Name, data switch { string value => value, _ => JsonConvert.SerializeObject(data) });
    }
}
