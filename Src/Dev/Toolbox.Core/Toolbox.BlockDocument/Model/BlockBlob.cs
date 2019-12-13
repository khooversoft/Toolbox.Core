using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Khooversoft.Toolbox.BlockDocument
{
    public class BlockBlob : IBlockData
    {
        public BlockBlob(string name, string contentType, string author, IReadOnlyList<byte> content)
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
            Content = content;

            Content = new List<byte>(content);
        }

        public string Name { get; }

        public string ContentType { get; }

        public string Author { get; }

        public IReadOnlyList<byte> Content { get; }

        public IReadOnlyList<byte> GetUTF8Bytes()
        {
            return Content.ToArray();
        }
    }
}
