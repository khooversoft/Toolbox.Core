using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Khooversoft.MessageNet.Interface
{
    public static class MessageNetExtensions
    {
        public static MessageHeader WithReply(this MessageHeader netMessage, string method, params MessageClaim[] claims)
        {
            netMessage.VerifyNotNull(nameof(netMessage));
            method.VerifyNotNull(nameof(method));

            return new MessageHeader(netMessage.FromUri, netMessage.ToUri, method, claims);
        }

        public static NetMessageBuilder ToBuilder(this NetMessage subject) => new NetMessageBuilder(subject);

        public static bool HasClaim(this NetMessage netMessage, MessageClaim messageClaim)
        {
            netMessage.VerifyNotNull(nameof(netMessage));
            messageClaim.VerifyNotNull(nameof(messageClaim));

            return netMessage.Header.Claims.Any(x => x == messageClaim);
        }

        public static IReadOnlyList<MessageClaim> GetClaim(this NetMessage netMessage, string role)
        {
            netMessage.VerifyNotNull(nameof(netMessage));

            return netMessage.Header.Claims
                .Where(x => x.IsRole(role))
                .ToList();
        }
    }
}
