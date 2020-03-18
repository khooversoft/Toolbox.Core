using Khooversoft.Toolbox.Actor;
using Khooversoft.Toolbox.Standard;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Khooversoft.MessageNet.Interface
{
    public static class ModelExtensions
    {
        // ========================================================================================
        // Message claim
        // ========================================================================================

        public static MessageClaimModel ConvertTo(this MessageClaim subject)
        {
            subject.Verify(nameof(subject)).IsNotNull();

            return new MessageClaimModel
            {
                Role = subject.Role,
                Value = subject.Value,
            };
        }

        public static MessageClaim ConvertTo(this MessageClaimModel subject)
        {
            subject.Verify(nameof(subject)).IsNotNull();

            return new MessageClaim(subject.Role!, subject.Value!);
        }

        // ========================================================================================
        // Message Header
        // ========================================================================================

        public static MessageHeaderModel ConvertTo(this MessageHeader subject)
        {
            subject.Verify(nameof(subject)).IsNotNull();

            return new MessageHeaderModel
            {
                MessageId = subject.MessageId,
                ToUri = subject.ToUri,
                FromUri = subject.FromUri,
                Method = subject.Method,
                Claims = subject.Claims.Select(x => x.ConvertTo()).ToList(),
            };
        }

        public static MessageHeader ConvertTo(this MessageHeaderModel subject)
        {
            subject.Verify(nameof(subject)).IsNotNull();

            return new MessageHeader(subject.MessageId, subject.ToUri!, subject.FromUri!, subject.Method!, subject.Claims.Select(x => x.ConvertTo()).ToArray());
        }

        // ========================================================================================
        // Message Activity
        // ========================================================================================

        public static MessageActivityModel ConvertTo(this MessageActivity subject)
        {
            subject.Verify(nameof(subject)).IsNotNull();

            return new MessageActivityModel
            {
                ActivityId = subject.ActivityId,
                ParentActivityId = subject.ParentActivityId,
            };
        }

        public static MessageActivity ConvertTo(this MessageActivityModel subject)
        {
            subject.Verify(nameof(subject)).IsNotNull();

            return new MessageActivity(subject.ActivityId, subject.ParentActivityId);
        }

        // ========================================================================================
        // Message content
        // ========================================================================================

        public static MessageContentModel ConvertTo(this MessageContent subject)
        {
            subject.Verify(nameof(subject)).IsNotNull();

            return new MessageContentModel
            {
                ContentType = subject.ContentType,
                Content = subject.Content,
            };
        }

        public static MessageContent ConvertTo(this MessageContentModel subject)
        {
            subject.Verify(nameof(subject)).IsNotNull();

            return new MessageContent(subject.ContentType!, subject.Content!);
        }

        // ========================================================================================
        // Net message
        // ========================================================================================

        public static NetMessageModel ConvertTo(this NetMessage subject)
        {
            subject.Verify(nameof(subject)).IsNotNull();

            return new NetMessageModel
            {
                Version = subject.Version,
                Headers = subject.Headers.Select(x => x.ConvertTo()).ToList(),
                Activities = subject.Activities.Select(x => x.ConvertTo()).ToList(),
                Contents = subject.Contents.Select(x => x.ConvertTo()).ToList(),
            };
        }

        public static NetMessage ConvertTo(this NetMessageModel subject)
        {
            subject.Verify(nameof(subject)).IsNotNull();

            var list = subject.Headers.Select(x => x.ConvertTo()).OfType<INetMessageItem>()
                .Concat(subject.Activities.Select(x => x.ConvertTo()).OfType<INetMessageItem>())
                .Concat(subject.Contents.Select(x => x.ConvertTo()).OfType<INetMessageItem>())
                .ToList();

            return new NetMessage(subject.Version!, list);
        }

        // ========================================================================================
        // To object
        // ========================================================================================

        public static T Deserialize<T>(this MessageContent subject) where T : class
        {
            return JsonConvert.DeserializeObject<T>(subject.Content);
        }
    }
}
