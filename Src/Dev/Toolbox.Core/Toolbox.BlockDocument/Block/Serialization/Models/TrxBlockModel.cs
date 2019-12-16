// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

namespace Khooversoft.Toolbox.BlockDocument
{
    public class TrxBlockModel : IDataBlockModelType
    {
        public string? ReferenceId { get; set; }

        public string? TransactionType { get; set; }

        public double Value { get; set; }
    }
}
