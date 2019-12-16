// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

namespace Khooversoft.Toolbox.BlockDocument
{
    public class BlockChainNodeModel
    {
        public int Index { get; set; }

        public string? PreviousHash { get; set; }

        public string? Hash { get; set; }

        public DataBlockModel<BlockBlobModel>? Blob { get; set; }

        public DataBlockModel<HeaderBlockModel>? Header { get; set; }

        public DataBlockModel<TrxBlockModel>? Trx { get; set; }

        public DataBlockModel<TextBlockModel>? Text { get; set; }
    }
}
