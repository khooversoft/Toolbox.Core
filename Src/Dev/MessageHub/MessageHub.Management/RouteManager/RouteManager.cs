// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.MessageHub.Interface;
using Khooversoft.Toolbox.Actor;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.MessageHub.Management
{
    public class RouteManager
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

            Uri uri = new ResourcePathBuilder()
                .SetScheme(ResourceScheme.Queue)
                .SetServiceBusName("Default")
                .SetEntityName(request.NodeId!)
                .Build();

            INodeRegistrationActor registgrationActor = await _actorManager.CreateProxy<INodeRegistrationActor>(request.NodeId!);
            await registgrationActor.Set(context, request.ConvertTo(uri));

            QueueDefinition queueDefinition = new QueueDefinition
            {
                QueueName = request.NodeId,
            };

            IQueueManagementActor queueActor = await _actorManager.CreateProxy<IQueueManagementActor>(request.NodeId!);
            await queueActor.Set(context, queueDefinition);

            return new RouteRegistrationResponse
            {
                InputQueueUri = uri.ToString(),
            };
        }

        /// <summary>
        /// Unregistered route
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="nodeId">node id</param>
        /// <returns>task</returns>
        public async Task Unregister(IWorkContext context, string nodeId)
        {
            nodeId.Verify(nameof(nodeId)).IsNotNull();

            Uri uri = new ResourcePathBuilder()
                .SetScheme(ResourceScheme.Queue)
                .SetServiceBusName("Default")
                .SetEntityName(nodeId)
                .Build();

            INodeRegistrationActor subject = await _actorManager.CreateProxy<INodeRegistrationActor>(nodeId);
            await subject.Remove(context);
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

            INodeRegistrationManagementActor managementActor = await _actorManager.CreateProxy<INodeRegistrationManagementActor>("default");
            IReadOnlyList<NodeRegistrationModel> registrations = await managementActor.List(context, request.SearchNodeId!);

            return registrations
                .Select(x => new RouteLookupResponse
                {
                    NodeId = x.NodeId,
                    InputUri = x.InputUri,
                })
                .ToList();
        }
    }
}
