// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Toolbox.BlockDocument
{
    public class HeaderBlock : IBlockType
    {
        public HeaderBlock(string description)
        {
            description.Verify(nameof(description)).IsNotEmpty();

            CreatedDate = DateTime.UtcNow;
            Description = description;
        }

        public HeaderBlock(DateTime createdDate, string description)
        {
            description.Verify(nameof(description)).IsNotEmpty();

            CreatedDate = createdDate;
            Description = description;
        }

        public DateTime CreatedDate { get; }

        public string Description { get; }

        public IReadOnlyList<byte> GetBytesForHash()
        {
            return Encoding.UTF8.GetBytes($"{CreatedDate.ToString("o")}-{Description}");
        }
    }
}
