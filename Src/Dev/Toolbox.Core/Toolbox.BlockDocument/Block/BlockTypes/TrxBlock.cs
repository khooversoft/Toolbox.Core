// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Security;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Toolbox.BlockDocument
{
    public class TrxBlock : IBlockType
    {
        public TrxBlock(string referenceId, string transactionType, MaskDecimal4 value)
        {
            referenceId.Verify(nameof(referenceId)).IsNotEmpty();
            transactionType.Verify(nameof(transactionType)).IsNotEmpty();

            ReferenceId = referenceId;
            TransactionType = transactionType;
            Value = value;

            Digest = GetDigest();
        }

        public string ReferenceId { get; }

        public string TransactionType { get; }

        public MaskDecimal4 Value { get; }

        // TODO: Need properties

        public string Digest { get; }

        public string GetDigest() => $"{ReferenceId}-{TransactionType}-{Value}"
                .ToBytes()
                .ToSHA256Hash();

        public override bool Equals(object obj)
        {
            if (obj is TrxBlock trxBlock)
            {
                return ReferenceId == trxBlock.ReferenceId &&
                    TransactionType == trxBlock.TransactionType &&
                    Value == trxBlock.Value &&
                    Digest == trxBlock.Digest;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return ReferenceId.GetHashCode() ^
                TransactionType.GetHashCode() ^
                Value.GetHashCode() ^
                Digest.GetHashCode();
        }

        public static bool operator ==(TrxBlock v1, TrxBlock v2) => v1.Equals(v2);

        public static bool operator !=(TrxBlock v1, TrxBlock v2) => !v1.Equals(v2);
    }
}
