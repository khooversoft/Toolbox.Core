// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Khooversoft.Toolbox.BlockDocument
{
    public static class BlockChainConvertExtensions
    {
        public static string ToJson(this BlockChain blockChain)
        {
            blockChain.Verify(nameof(blockChain)).IsNotNull();
            blockChain.Chain.Count.Verify(nameof(blockChain.Chain.Count)).Assert<int, InvalidOperationException>(x => x > 1, "Empty block chain");

            var list = new List<BlockChainNodeModel>();

            foreach (BlockNode node in blockChain)
            {
                BlockChainNodeModel dataBlockNodeModel = node.ConvertTo();
                list.Add(dataBlockNodeModel);
            }

            var blockChainModel = new BlockChainModel()
            {
                Blocks = list,
            };

            return JsonSerializer.Serialize(blockChainModel);
        }

        public static BlockChain ToBlockChain(this string json)
        {
            json.Verify(nameof(json)).IsNotEmpty();

            var blockChainModel = JsonSerializer.Deserialize<BlockChainModel>(json);
            blockChainModel.Blocks.Verify(nameof(blockChainModel.Blocks)).IsNotNull();

            var list = new List<BlockNode>();

            foreach (var node in blockChainModel.Blocks!)
            {
                BlockNode blockNode = node.ConvertTo();
                list.Add(blockNode);
            }

            return new BlockChain(list);
        }
    }
}
