// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Khooversoft.Toolbox.BlockDocument
{
    public class TextBlock : IBlockType
    {
        public TextBlock(string name, string contentType, string author, string content)
        {
            name.Verify(nameof(name)).IsNotEmpty();
            contentType.Verify(nameof(contentType)).IsNotEmpty();
            author.Verify(nameof(author)).IsNotEmpty();
            content.Verify(nameof(content)).IsNotEmpty();

            Name = name;
            ContentType = contentType;
            Author = author;
            Content = content;
        }

        public string Name { get; }

        public string ContentType { get; }

        public string Author { get; }

        public string Content { get; }

        public IReadOnlyList<byte> GetBytesForHash()
        {
            return Encoding.UTF8.GetBytes($"{Name}-{ContentType}-{Author}-{Content}");
        }

        public override bool Equals(object obj)
        {
            if (obj is TextBlock textBlock)
            {
                return Name == textBlock.Name &&
                    ContentType == textBlock.ContentType &&
                    Author == textBlock.Author &&
                    Content == textBlock.Content;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() ^
                ContentType.GetHashCode() ^
                Author.GetHashCode() ^
                Content.GetHashCode();
        }

        public static bool operator ==(TextBlock v1, TextBlock v2) =>v1.Equals(v2);

        public static bool operator !=(TextBlock v1, TextBlock v2) => !v1.Equals(v2);
    }
}
