// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.MessageNet.Interface
{
    public abstract class MessageContent : INetMessageItem
    {
        protected MessageContent(string contentType, string content)
        {
            ContentType = contentType.Verify(nameof(contentType)).IsNotEmpty().Value;
            Content = content.Verify(nameof(content)).IsNotEmpty().Value;
        }

        public string ContentType { get; }

        public string Content { get; }

        public override bool Equals(object obj)
        {
            if( obj is MessageContent subject)
            {
                return ContentType == subject.ContentType &&
                    Content == subject.Content;
            }

            return false;
        }

        public override int GetHashCode() => HashCode.Combine(ContentType, Content);

        public static bool operator ==(MessageContent v1, MessageContent V2) => v1?.Equals(V2) == true;

        public static bool operator !=(MessageContent v1, MessageContent v2) => !v1.Equals(v2) == false;
    }

    public class MessageContent<T> : MessageContent
    {
        public MessageContent(T data)
            : base(typeof(T).Name, data switch { string value => value, _ => JsonConvert.SerializeObject(data) })
        {
        }
    }
}
