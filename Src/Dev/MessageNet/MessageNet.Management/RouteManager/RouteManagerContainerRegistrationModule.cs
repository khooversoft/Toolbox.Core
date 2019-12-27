using Khooversoft.Toolbox.Actor;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.MessageNet.Management
{
    public class RouteManagerContainerRegistrationModule : ContainerRegistrationModule
    {
        public RouteManagerContainerRegistrationModule()
        {
            Add(typeof(NodeRegistrationActor), typeof(INodeRegistrationActor));
            Add(typeof(QueueManagementActor), typeof(IQueueManagementActor));
            Add(typeof(NodeRegistrationManagementActor), typeof(INodeRegistrationManagementActor));
            Add(typeof(RouteManager), typeof(IRouteManager));

            Add(typeof(ActorManager), typeof(IActorManager));
        }
    }
}
