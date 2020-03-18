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
            netMessage.Verify(nameof(netMessage)).IsNotNull();
            method.Verify(nameof(method)).IsNotEmpty();

            return new MessageHeader(netMessage.FromUri, netMessage.ToUri, method, claims);
        }

        public static NetMessageBuilder ToBuilder(this NetMessage subject)
        {
            return new NetMessageBuilder(subject);
        }

        public static bool HasClaim(this NetMessage netMessage, MessageClaim messageClaim)
        {
            netMessage.Verify(nameof(netMessage)).IsNotNull();
            messageClaim.Verify(nameof(messageClaim)).IsNotNull();

            return netMessage.Header.Claims.Any(x => x == messageClaim);
        }

        public static IReadOnlyList<MessageClaim> GetClaim(this NetMessage netMessage, string role)
        {
            netMessage.Verify(nameof(netMessage)).IsNotNull();

            return netMessage.Header.Claims
                .Where(x => x.IsRole(role))
                .ToList();
        }
    }
}
