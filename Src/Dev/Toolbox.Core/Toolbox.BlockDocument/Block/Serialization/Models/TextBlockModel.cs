// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

namespace Khooversoft.Toolbox.BlockDocument
{
    public class TextBlockModel : IDataBlockModelType
    {
        public string? Name { get; set; }

        public string? ContentType { get; set; }

        public string? Author { get; set; }

        public string? Content { get; set; }
    }
}
