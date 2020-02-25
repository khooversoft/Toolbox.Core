using Khooversoft.Toolbox.Actor;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.MessageNet.Interface
{
    public static class RouteExtensions
    {
        public static bool IsValid(this RouteRequest subject) => QueueId.IsValid(subject?.NetworkId, subject?.NodeId);

        public static bool IsValid(this RouteResponse subject) => QueueId.IsValid(subject?.Namespace, subject?.NetworkId, subject?.NodeId);

        public static ActorKey ToActorKey(this RouteRequest subject) => subject
            .Verify()
            .Assert(x => x.IsValid(), $"{nameof(RouteRequest)} is not valid")
            .Value
            .Do(x => new ActorKey(x.NetworkId + "/" + x.NodeId));
    }
}
