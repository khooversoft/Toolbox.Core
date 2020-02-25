using Khooversoft.Toolbox.Actor;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.MessageNet.Interface
{
    public static class ModelExtensions
    {
        public static NodeRegistrationStoreModel ConvertTo(this NodeRegistration subject)
        {
            subject.Verify(nameof(subject)).IsNotNull();

            return new NodeRegistrationStoreModel
            {
                Namespace = subject.Namespace,
                NetworkId = subject.QueueId.NetworkId,
                NodeId = subject.QueueId.NodeId,
            };
        }

        public static QueueId ConvertTo(this NodeRegistrationStoreModel subject)
        {
            subject.Verify(nameof(subject)).IsNotNull();
            subject.Namespace!.Verify(nameof(subject.Namespace)).IsNotEmpty();
            subject.NetworkId!.Verify(nameof(subject.NetworkId)).IsNotEmpty();
            subject.NodeId!.Verify(nameof(subject.NodeId)).IsNotEmpty();

            return new QueueId(subject.Namespace!, subject.NetworkId!, subject.NodeId!);
        }

        public static ActorKey ToActorKey(this QueueId subject) => new ActorKey(subject.Verify().IsNotNull().Value.ToString());

        public static QueueId ToQueueId(this ActorKey subject) => QueueId.Parse(subject.VectorKey);

        public static QueueId ToQueueId(this MessageUri subject) => subject.Verify().IsNotNull().Value.Do(x => new QueueId(x.NetworkId, x.NodeId));
    }
}
