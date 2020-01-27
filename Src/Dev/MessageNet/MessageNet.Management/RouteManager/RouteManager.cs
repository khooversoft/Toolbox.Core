// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Actor;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.MessageNet.Management
{
    public class RouteManager : IRouteManager
    {
        private readonly IActorManager _actorManager;

        public RouteManager(IActorManager actorManager)
        {
            actorManager.Verify(nameof(actorManager)).IsNotNull();
            _actorManager = actorManager;
        }

        /// <summary>
        /// Register Node by NodeId
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="request">request</param>
        /// <returns>response</returns>
        public async Task<RouteRegistrationResponse> Register(IWorkContext context, RouteRegistrationRequest request)
        {
            request.Verify(nameof(request)).IsNotNull();
            request.NodeId.Verify(nameof(request.NodeId)).IsNotNull();

            INodeRegistrationActor registgrationActor = await _actorManager.GetActor<INodeRegistrationActor>(request.NodeId!);
            await registgrationActor.Set(context, request.ConvertTo(request.NodeId!));

            QueueDefinition queueDefinition = new QueueDefinition
            {
                QueueName = request.NodeId,
            };

            IQueueManagementActor queueActor = await _actorManager.GetActor<IQueueManagementActor>(request.NodeId!);
            await queueActor.Set(context, queueDefinition);

            return new RouteRegistrationResponse
            {
                InputQueueUri = request.NodeId,
            };
        }

        /// <summary>
        /// Unregistered route
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="nodeId">node id</param>
        /// <returns>task</returns>
        public async Task Unregister(IWorkContext context, RouteRegistrationRequest routeRegistrationRequest)
        {
            routeRegistrationRequest.Verify(nameof(routeRegistrationRequest)).IsNotNull();
            routeRegistrationRequest.NodeId.Verify(nameof(routeRegistrationRequest.NodeId)).IsNotNull();

            INodeRegistrationActor subject = await _actorManager.GetActor<INodeRegistrationActor>(routeRegistrationRequest.NodeId!);
            await subject.Remove(context);

            IQueueManagementActor queueActor = await _actorManager.GetActor<IQueueManagementActor>(routeRegistrationRequest.NodeId!);
            await queueActor.Remove(context);
        }

        /// <summary>
        /// Search for node registration
        /// </summary>
        /// <param name="context"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<IReadOnlyList<RouteLookupResponse>> Search(IWorkContext context, RouteLookupRequest request)
        {
            request.Verify(nameof(request)).IsNotNull();

            INodeRegistrationManagementActor managementActor = await _actorManager.GetActor<INodeRegistrationManagementActor>("default");
            IReadOnlyList<NodeRegistrationModel> registrations = await managementActor.List(context, request.NodeId!);

            return registrations
                .Select(x => new RouteLookupResponse
                {
                    NodeId = x.NodeId,
                    InputUri = x.InputQueueUri,
                })
                .ToList();
        }

        /// <summary>
        /// Clear all stores and actors
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Clear(IWorkContext context)
        {
            INodeRegistrationManagementActor managementActor = await _actorManager.GetActor<INodeRegistrationManagementActor>("default");
            await managementActor.ClearRegistery(context);
        }
    }
}
