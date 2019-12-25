// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Khooversoft.Toolbox.BlockDocument
{
    public static class BlockChainConvertExtensions
    {
        public static string ToJson(this BlockChain blockChain)
        {
            blockChain.Verify(nameof(blockChain)).IsNotNull();
            blockChain.Blocks.Count.Verify(nameof(blockChain.Blocks.Count)).Assert<int, InvalidOperationException>(x => x > 1, "Empty block chain");

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

            return JsonConvert.SerializeObject(blockChainModel);
        }

        public static BlockChain ToBlockChain(this string json)
        {
            json.Verify(nameof(json)).IsNotEmpty();

            var blockChainModel = JsonConvert.DeserializeObject<BlockChainModel>(json);
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
