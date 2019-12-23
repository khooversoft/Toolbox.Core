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

        public static void Validate(this IDataBlock subject, IWorkContext context, PrincipleSignature principleSignature)
        {
            subject.Verify(nameof(subject)).IsNotNull();
            principleSignature.Verify(nameof(principleSignature)).IsNotNull();

            subject.Validate();

            JwtTokenDetails? tokenDetails = principleSignature.ValidateSignature(context, subject.JwtSignature!);
            tokenDetails.Verify().IsNotNull("Signature validation failed");

            string digest = tokenDetails!.Claims.Where(x => x.Type == "blockDigest").Select(x => x.Value).FirstOrDefault();
            (digest == subject.Digest).Verify().Assert<bool, SecurityException>(x => x == true, "Block's digest does not match signature");
        }

        public static void Validate(this BlockChain blockChain, IWorkContext context, IPrincipleSignatureContainer keyContainer)
        {
            blockChain.Verify(nameof(blockChain)).IsNotNull();
            keyContainer.Verify(nameof(keyContainer)).IsNotNull();

            blockChain.IsValid()
                .Verify()
                .Assert<bool, SecurityException>(x => x == true, "Block chain has linkage is invalid");

            foreach(var node in blockChain.Blocks)
            {
                // Skip header
                if (node.Index == 0) continue;

                string? issuer = JwtTokenParser.GetIssuerFromJwtToken(node.BlockData.JwtSignature!)!
                    .Verify()
                    .Assert<string, SecurityException>(x => x != null, "Issuer is not found in JWT Signature")
                    .Value;

                PrincipleSignature principleSignature = keyContainer.Get(issuer!)!;
                principleSignature.Verify().IsNotNull("Signature for issuer {issuer} is not in container");

                node.BlockData.Validate(context, principleSignature);
            }
        }
    }
}
