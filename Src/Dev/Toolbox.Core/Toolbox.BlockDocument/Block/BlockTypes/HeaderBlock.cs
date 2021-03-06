﻿// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Security;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Toolbox.BlockDocument
{
    public class HeaderBlock : IBlockType
    {
        public HeaderBlock(string description)
            : this(UnixDate.UtcNow, description)
        {
        }

        public HeaderBlock(UnixDate timeStamp, string description)
        {
            description.Verify(nameof(description)).IsNotEmpty();

            TimeStamp = timeStamp;
            Description = description;

            Digest = GetDigest();
        }

        public UnixDate TimeStamp { get; }

        public string Description { get; }

        public string Digest { get; }

        public string GetDigest() => $"{TimeStamp}-{Description}"
                .ToBytes()
                .ToSHA256Hash();

        public override bool Equals(object obj)
        {
            if (obj is HeaderBlock headerBlock)
            {
                return TimeStamp == headerBlock.TimeStamp &&
                    Description == headerBlock.Description &&
                    Digest == headerBlock.Digest;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return TimeStamp.GetHashCode() ^
                Description.GetHashCode() ^
                Digest.GetHashCode();
        }

        public static bool operator ==(HeaderBlock v1, HeaderBlock v2) => v1.Equals(v2);

        public static bool operator !=(HeaderBlock v1, HeaderBlock v2) => !v1.Equals(v2);
    }
}
