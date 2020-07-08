// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Security;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Khooversoft.Toolbox.BlockDocument
{
    public static class DataBlockExtensions
    {
        public static DataBlock<T> WithSignature<T>(this DataBlock<T> subject, IPrincipleSignature principleSign)
            where T : IBlockType
        {
            return new DataBlock<T>(subject, principleSign);
        }

        public static void Validate(this IDataBlock subject, PrincipleSignature principleSignature)
        {
            subject.VerifyNotNull(nameof(subject));
            principleSignature.VerifyNotNull(nameof(principleSignature));

            subject.Validate();

            JwtTokenDetails? tokenDetails = principleSignature.ValidateSignature(subject.JwtSignature!);
            tokenDetails.VerifyNotNull("Signature validation failed");

            string digest = tokenDetails!.Claims.Where(x => x.Type == "blockDigest").Select(x => x.Value).FirstOrDefault();
            (digest == subject.Digest).VerifyAssert<bool, SecurityException>(x => x == true, _ => "Block's digest does not match signature");
        }

        public static void Validate(this BlockChain blockChain, IPrincipleSignatureContainer keyContainer)
        {
            blockChain.VerifyNotNull(nameof(blockChain));
            keyContainer.VerifyNotNull(nameof(keyContainer));

            blockChain.IsValid()
                .VerifyAssert<bool, SecurityException>(x => x == true, _ => "Block chain has linkage is invalid");

            foreach (var node in blockChain.Blocks)
            {
                // Skip header
                if (node.Index == 0) continue;

                string? issuer = JwtTokenParser.GetIssuerFromJwtToken(node.BlockData.JwtSignature!)!
                    .VerifyAssert<string, SecurityException>(x => x != null, _ => "Issuer is not found in JWT Signature");

                PrincipleSignature principleSignature = keyContainer.Get(issuer!)!;
                principleSignature.VerifyNotNull("Signature for issuer {issuer} is not in container");

                node.BlockData.Validate(principleSignature);
            }
        }
    }
}
