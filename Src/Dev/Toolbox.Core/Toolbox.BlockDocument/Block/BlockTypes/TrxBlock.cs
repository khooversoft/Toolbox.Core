// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Toolbox.BlockDocument
{
    public class TrxBlock : IBlockType
    {
        public TrxBlock(string referenceId, string transactionType, double value)
        {
            referenceId.Verify(nameof(referenceId)).IsNotEmpty();
            transactionType.Verify(nameof(transactionType)).IsNotEmpty();

            ReferenceId = referenceId;
            TransactionType = transactionType;
            Value = value;
        }

        public string ReferenceId { get; }

        public string TransactionType { get; }

        public double Value { get; }

        public IReadOnlyList<byte> GetBytesForHash()
        {
            return Encoding.UTF8.GetBytes($"{ReferenceId}-{TransactionType}-{Value}");
        }
    }
}
