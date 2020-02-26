using Khooversoft.Toolbox.Actor;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
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
    }
}
