using Khooversoft.Toolbox.Actor;
using Khooversoft.Toolbox.Standard;
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
            return new QueueId(subject.Namespace!, subject.NetworkId!, subject.NodeId!);
        }

        public static QueueId ToQueueId(this MessageUri subject) => subject.Verify().IsNotNull().Value.Do(x => new QueueId(x.Namespace, x.NetworkId, x.NodeId));

        public static MessageHeaderModel ConvertTo(this MessageHeader subject)
        {
            return new MessageHeaderModel
            {
                MessageId = subject.MessageId,
                ToUri = subject.ToUri,
                FromUri = subject.FromUri,
                Method = subject.Method,
                Properties = subject.Properties.ToDictionary(x => x.Key, x => x.Value, StringComparer.OrdinalIgnoreCase),
            };
        }

        public static MessageHeader ConvertTo(this MessageHeaderModel subject)
        {
            return new MessageHeader(subject.ToUri!, subject.FromUri!, subject.Method!, subject.Properties.ToArray());
        }

        public static MessageActivityModel ConvertTo(this MessageActivity subject)
        {
            return new MessageActivityModel
            {
                ActivityId = subject.ActivityId,
                ParentActivityId = subject.ParentActivityId,
            };
        }

        public static MessageActivity ConvertTo(this MessageActivityModel subject)
        {
            return new MessageActivity(subject.ActivityId, subject.ParentActivityId);
        }

        public static MessageContentModel ConvertTo(this MessageContent subject)
        {
            return new MessageContentModel
            {
                ContentType = subject.ContentType,
                Content = subject.Content,
            };
        }

        public static MessageContent ConvertTo(this MessageContentModel subject)
        {
            return new MessageContent(subject.ContentType!, subject.Content!);
        }
    }
}
