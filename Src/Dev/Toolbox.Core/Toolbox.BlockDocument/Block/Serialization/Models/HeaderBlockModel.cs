// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Khooversoft.Toolbox.BlockDocument
{
    public class HeaderBlockModel : IDataBlockModelType
    {
        public DateTime CreatedDate { get; set; }

        public string? Description { get; set; }
    }
}
