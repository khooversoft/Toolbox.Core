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
    public class RouteRepository : IRouteRepository
    {
        private readonly IActorManager _actorManager;

        public RouteRepository(IActorManager actorManager)
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
        public async Task<QueueId> Register(IWorkContext context, RouteRequest request)
        {
            request.Verify(nameof(request)).Assert(x => x.IsValid(), "Route request is invalid");

            var actorKey = request.ToActorKey();

            NodeRegistration nodeRegistration = await _actorManager.GetActor<INodeRegistrationActor>(actorKey)
                .Set(context);

            QueueDefinition queueDefinition = new QueueDefinition
            {
                QueueName = actorKey.ToString(),
            };

            await _actorManager.GetActor<IQueueManagementActor>(actorKey)
                .Set(context, queueDefinition);

            return nodeRegistration;
        }

        /// <summary>
        /// Unregistered route
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="nodeId">node id</param>
        /// <returns>task</returns>
        public async Task Unregister(IWorkContext context, RouteRequest request)
        {
            request.Verify(nameof(request)).IsNotNull();

            var actorKey = request.ToActorKey();

            await _actorManager.GetActor<INodeRegistrationActor>(actorKey)
                .Remove(context);

            await _actorManager.GetActor<IQueueManagementActor>(actorKey)
                .Remove(context);
        }

        /// <summary>
        /// Search for node registration
        /// </summary>
        /// <param name="context"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<IReadOnlyList<QueueId>> Search(IWorkContext context, RouteRequest request)
        {
            request.Verify(nameof(request)).IsNotNull();

            return await _actorManager.GetActor<INodeRegistrationManagementActor>("default")
                .List(context, request.NodeId!);
        }

        /// <summary>
        /// Clear all stores and actors
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Clear(IWorkContext context)
        {
            await _actorManager.GetActor<INodeRegistrationManagementActor>("default")
                .ClearRegistery(context);
        }
    }
}
