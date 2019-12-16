// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Khooversoft.Toolbox.BlockDocument
{
    public static class SerializationConvertToExtensions
    {
        public static BlockChainNodeModel ConvertTo(this BlockNode subject)
        {
            subject.Verify(nameof(subject)).IsNotNull();

            return new BlockChainNodeModel
            {
                Index = subject.Index,
                PreviousHash = subject.PreviousHash,
                Hash = subject.Hash,
                Header = subject.BlockData switch { DataBlock<HeaderBlock> headerBlock => headerBlock.ConvertTo<HeaderBlock, HeaderBlockModel>(), _ => null },
                Trx = subject.BlockData switch { DataBlock<TrxBlock> headerBlock => headerBlock.ConvertTo<TrxBlock, TrxBlockModel>(), _ => null },
                Blob = subject.BlockData switch { DataBlock<BlockBlob> headerBlock => headerBlock.ConvertTo<BlockBlob, BlockBlobModel>(), _ => null },
                Text = subject.BlockData switch { DataBlock<TextBlock> headerBlock => headerBlock.ConvertTo<TextBlock, TextBlockModel>(), _ => null },
            };
        }

        public static BlockNode ConvertTo(this BlockChainNodeModel subject)
        {
            subject.Verify(nameof(subject)).IsNotNull();

            var dataBlockList = new List<IDataBlock>();

            if (subject.Header != null) dataBlockList.Add(subject.Header.ConvertTo<HeaderBlockModel, HeaderBlock>());
            if (subject.Trx != null) dataBlockList.Add(subject.Trx.ConvertTo<TrxBlockModel, TrxBlock>());
            if (subject.Blob != null) dataBlockList.Add(subject.Blob.ConvertTo<BlockBlobModel, BlockBlob>());
            if (subject.Text != null) dataBlockList.Add(subject.Text.ConvertTo<TextBlockModel, TextBlock>());

            dataBlockList.Count.Verify().Assert<int, InvalidOperationException>(x => x == 1, $"{nameof(BlockChainNodeModel)} has zero or more then 1 block type specified");

            return new BlockNode(dataBlockList.First(), subject.Index, subject.PreviousHash);
        }

        // ========================================================================================
        // Data block
        //public static BlockChainNodeModel ConvertTo(this IDataBlock subject)
        //{
        //    subject.Verify(nameof(subject)).IsNotNull();

        //    BlockChainNodeModel model = subject switch
        //    {
        //        DataBlock<HeaderBlock> headerBlock => new BlockChainNodeModel { Header = headerBlock.ConvertTo<HeaderBlock, HeaderBlockModel>() },
        //        DataBlock<TrxBlock> trxBlock => new BlockChainNodeModel { Trx = trxBlock.ConvertTo<TrxBlock, TrxBlockModel>() },
        //        DataBlock<BlockBlob> blockBlob => new BlockChainNodeModel { Blob = blockBlob.ConvertTo<BlockBlob, BlockBlobModel>() },
        //        DataBlock<TextBlock> textBlock => new BlockChainNodeModel { Text = textBlock.ConvertTo<TextBlock, TextBlockModel>() },

        //        _ => throw new InvalidOperationException($"Unknown type {subject.GetType().Name}"),
        //    };

        //    return model;
        //}

        //public static IDataBlock ConvertTo(this BlockChainNodeModel subject)
        //{
        //    subject.Verify(nameof(subject)).IsNotNull();

        //    var dataBlockList = new List<IDataBlock>();

        //    if (subject.Header != null) dataBlockList.Add(subject.Header.ConvertTo<HeaderBlockModel, HeaderBlock>());
        //    if (subject.Trx != null) dataBlockList.Add(subject.Trx.ConvertTo<TrxBlockModel, TrxBlock>());
        //    if (subject.Blob != null) dataBlockList.Add(subject.Blob.ConvertTo<BlockBlobModel, BlockBlob>());
        //    if (subject.Text != null) dataBlockList.Add(subject.Text.ConvertTo<TextBlockModel, TextBlock>());

        //    dataBlockList.Count.Verify().Assert<int, InvalidOperationException>(x => x == 1, $"{nameof(BlockChainNodeModel)} has zero or more then 1 block type specified");

        //    return dataBlockList.First();
        //}

        // ========================================================================================
        // Data block
        public static DataBlockModel<TTarget> ConvertTo<TSource, TTarget>(this DataBlock<TSource> subject)
            where TSource : IBlockType
            where TTarget : IDataBlockModelType
        {
            subject.Verify(nameof(subject)).IsNotNull();

            IDataBlockModelType model = subject.Data switch
            {
                TextBlock textBlock => textBlock.ConvertTo(),
                BlockBlob blockBob => blockBob.ConvertTo(),
                HeaderBlock headerBlock => headerBlock.ConvertTo(),
                TrxBlock trxBlock => trxBlock.ConvertTo(),

                _ => throw new InvalidOperationException($"Unknown type {subject.Data.GetType().Name}"),
            };

            return new DataBlockModel<TTarget>
            {
                TimeStamp = subject.TimeStamp,
                BlockType = subject.BlockType,
                BlockId = subject.BlockId,
                Data = (TTarget)model,
                Properties = subject.Properties.ToDictionary(x => x.Key, x => x.Value),
            };
        }

        public static DataBlock<TTarget> ConvertTo<TSource, TTarget>(this DataBlockModel<TSource> subject)
            where TSource : IDataBlockModelType
            where TTarget : IBlockType
        {
            subject.Verify(nameof(subject)).IsNotNull();

            IBlockType block = subject.Data switch
            {
                TextBlockModel textBlockModel => textBlockModel.ConvertTo(),
                BlockBlobModel blockBlobModel => blockBlobModel.ConvertTo(),
                HeaderBlockModel headerBlockModel => headerBlockModel.ConvertTo(),
                TrxBlockModel trxBlockModel => trxBlockModel.ConvertTo(),

                _ => throw new InvalidOperationException($"Unknown type {subject.Data.GetType().Name}"),
            };

            return new DataBlock<TTarget>(subject.TimeStamp, subject.BlockType!, subject.BlockId!, (TTarget)block, subject.Properties);
        }

        // ========================================================================================
        // Header block
        public static HeaderBlockModel ConvertTo(this HeaderBlock subject)
        {
            subject.Verify(nameof(subject)).IsNotNull();

            return new HeaderBlockModel
            {
                CreatedDate = subject.CreatedDate,
                Description = subject.Description,
            };
        }

        public static HeaderBlock ConvertTo(this HeaderBlockModel subject)
        {
            subject.Verify(nameof(subject)).IsNotNull();

            return new HeaderBlock(subject.CreatedDate, subject.Description!);
        }

        // ========================================================================================
        // Blob block
        public static BlockBlobModel ConvertTo(this BlockBlob subject)
        {
            subject.Verify(nameof(subject)).IsNotNull();

            return new BlockBlobModel
            {
                Name = subject.Name,
                ContentType = subject.ContentType,
                Author = subject.Author,
                Content = subject.Content.ToList(),
            };
        }

        public static BlockBlob ConvertTo(this BlockBlobModel subject)
        {
            subject.Verify(nameof(subject)).IsNotNull();

            return new BlockBlob(subject.Name!, subject.ContentType!, subject.Author!, subject.Content!);
        }

        // ========================================================================================
        // Trx Block
        public static TrxBlockModel ConvertTo(this TrxBlock subject)
        {
            subject.Verify(nameof(subject)).IsNotNull();

            return new TrxBlockModel
            {
                ReferenceId = subject.ReferenceId,
                TransactionType = subject.TransactionType,
                Value = subject.Value,
            };
        }

        public static TrxBlock ConvertTo(this TrxBlockModel subject)
        {
            subject.Verify(nameof(subject)).IsNotNull();

            return new TrxBlock(subject.ReferenceId!, subject.TransactionType!, subject.Value);
        }


        // ========================================================================================
        // Text block
        public static TextBlockModel ConvertTo(this TextBlock subject)
        {
            subject.Verify(nameof(subject)).IsNotNull();

            return new TextBlockModel
            {
                Name = subject.Name,
                ContentType = subject.ContentType,
                Author = subject.Author,
                Content = subject.Content,
            };
        }

        public static TextBlock ConvertTo(this TextBlockModel subject)
        {
            subject.Verify(nameof(subject)).IsNotNull();

            return new TextBlock(subject.Name!, subject.ContentType!, subject.Author!, subject.Content!);
        }
    }
}
