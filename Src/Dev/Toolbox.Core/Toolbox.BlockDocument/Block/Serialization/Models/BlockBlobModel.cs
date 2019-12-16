// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Khooversoft.Toolbox.BlockDocument
{
    public class BlockBlobModel : IDataBlockModelType
    {
        public string? Name { get; set; }

        public string? ContentType { get; set; }

        public string? Author { get; set; }

        public IReadOnlyList<byte>? Content { get; set; }
    }
}
