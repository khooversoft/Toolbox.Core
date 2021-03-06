﻿// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Khooversoft.Toolbox.BlockDocument
{
    public interface IDataBlock
    {
        string GetDigest();

        public string Digest { get; }

        public string? JwtSignature { get; }

        void Validate();
    }
}