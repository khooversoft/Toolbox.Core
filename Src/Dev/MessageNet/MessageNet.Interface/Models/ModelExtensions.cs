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
        public static QueueIdStore ConvertTo(this QueueId subject)
        {
            subject.Verify(nameof(subject)).IsNotNull();

            return new QueueIdStore
            {
                Namespace = subject.Namespace,
                NetworkId = subject.NetworkId,
                NodeId = subject.NodeId,
            };
        }

        public static QueueId ConvertTo(this QueueIdStore subject)
        {
            subject.Verify(nameof(subject)).IsNotNull();
            
            return new QueueId(subject.Namespace!, subject.NetworkId!, subject.NodeId!);
        }

        public static QueueId ToQueueId(this MessageUri subject) => subject.Verify().IsNotNull().Value.Do(x => new QueueId(x.Namespace, x.NetworkId, x.NodeId));

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
                Properties = subject.Claims.ToDictionary(x => x.Key, x => x.Value, StringComparer.OrdinalIgnoreCase),
            };
        }

        public static MessageHeader ConvertTo(this MessageHeaderModel subject)
        {
            subject.Verify(nameof(subject)).IsNotNull();
            
            return new MessageHeader(subject.ToUri!, subject.FromUri!, subject.Method!, subject.Properties.ToArray());
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
