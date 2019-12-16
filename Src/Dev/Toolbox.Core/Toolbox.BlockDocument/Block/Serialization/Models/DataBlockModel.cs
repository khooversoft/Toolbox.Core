// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Toolbox.BlockDocument
{
    public class DataBlockModel<T> : IDataBlockModel
        where T : IDataBlockModelType
    {
        public DateTime TimeStamp { get; set; }

        public string? BlockType { get; set; }

        public string? BlockId { get; set; }

        public T Data { get; set; } = default!;

        public IReadOnlyDictionary<string, string>? Properties { get; set; }
    }
}
