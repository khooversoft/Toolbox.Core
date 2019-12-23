// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Security;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Khooversoft.Toolbox.BlockDocument
{
    public class BlobBlock : IBlockType
    {
        public BlobBlock(string name, string contentType, string author, IReadOnlyList<byte> content)
        {
            name.Verify(nameof(name)).IsNotEmpty();
            contentType.Verify(nameof(contentType)).IsNotEmpty();
            author.Verify(nameof(author)).IsNotEmpty();

            content.Verify(nameof(content))
                .IsNotNull()
                .Assert(x => x.Count > 0, "Content must have content");

            Name = name;
            ContentType = contentType;
            Author = author;
            Content = new List<byte>(content);

            Digest = GetDigest();
        }

        public string Name { get; }

        public string ContentType { get; }

        public string Author { get; }

        public IReadOnlyList<byte> Content { get; }

        public string Digest { get; }

        public string GetDigest() => $"{Name}-{ContentType}-{Author}-{Content.ToArray().Do(Convert.ToBase64String)}"
                .ToBytes()
                .Concat(Content)
                .ToSHA256Hash();

        public override bool Equals(object obj)
        {
            if (obj is BlobBlock blobBlock)
            {
                return Name == blobBlock.Name &&
                    ContentType == blobBlock.ContentType &&
                    Author == blobBlock.Author &&
                    Enumerable.SequenceEqual(Content, blobBlock.Content) &&
                    Digest == blobBlock.Digest;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() ^
                ContentType.GetHashCode() ^
                Author.GetHashCode() ^
                Content.GetHashCode() ^
                Digest.GetHashCode();
        }

        public static bool operator ==(BlobBlock v1, BlobBlock v2) => v1.Equals(v2);

        public static bool operator !=(BlobBlock v1, BlobBlock v2) => !v1.Equals(v2);
    }
}
