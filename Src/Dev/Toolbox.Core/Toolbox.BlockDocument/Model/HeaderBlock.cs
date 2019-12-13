using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Toolbox.BlockDocument
{
    public class HeaderBlock : IBlockData
    {
        public HeaderBlock(string description)
        {
            description.Verify(nameof(description)).IsNotEmpty();

            CreatedDate = DateTime.UtcNow;

            Description = description;
        }

        public DateTime CreatedDate { get; }

        public string Description { get; }

        public IReadOnlyList<byte> GetUTF8Bytes()
        {
            return Encoding.UTF8.GetBytes(Description);
        }
    }
}
