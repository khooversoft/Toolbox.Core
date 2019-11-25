using Khooversoft.Toolbox.Actor;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageHub.Management
{
    public class RouteManager
    {
        private readonly IQueueManagement _queueManagement;
        private readonly IRegisterStore _registerStore;
        private readonly IActorManager _actorManager;
        private readonly Deferred _deferredRegister;

        public RouteManager(IQueueManagement queueManagement, IRegisterStore registerStore)
        {
            queueManagement.Verify(nameof(queueManagement)).IsNotNull();
            registerStore.Verify(nameof(registerStore)).IsNotNull();

            _queueManagement = queueManagement;
            _registerStore = registerStore;
            _actorManager = new ActorManager();

            _deferredRegister = new Deferred(x => _actorManager.Register<INodeRegistrationActor>(x, c => new NodeRegistrationActor(_registerStore)));
        }

        /// <summary>
        /// Register Node by NodeId
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="request">request</param>
        /// <returns></returns>
        public async Task<RouteRegistrationResponse> Register(IWorkContext context, RouteRegistrationRequest request)
        {
            request.Verify(nameof(request)).IsNotNull();
            request.NodeId.Verify(nameof(request.NodeId)).IsNotNull();

            _deferredRegister.Execute(context);

            Uri uri = new ResourcePathBuilder()
                .SetScheme(ResourceScheme.Queue)
                .SetServiceBusName("Default")
                .SetEntityName(request.NodeId!)
                .Build();

            INodeRegistrationActor subject = await _actorManager.CreateProxy<INodeRegistrationActor>(context, uri.ToString());
            await subject.Set(context, request.ConvertTo());

            return new RouteRegistrationResponse
            {
                InputQueueUri = uri.ToString(),
            };
        }

        public async Task Unregister(IWorkContext context, string nodeId)
        {
            nodeId.Verify(nameof(nodeId)).IsNotNull();

            Uri uri = new ResourcePathBuilder()
                .SetScheme(ResourceScheme.Queue)
                .SetServiceBusName("Default")
                .SetEntityName(nodeId)
                .Build();

            INodeRegistrationActor subject = await _actorManager.CreateProxy<INodeRegistrationActor>(context, uri.ToString());
            await subject.Remove(context);
        }

        public Task<IReadOnlyList<QueueRegistration>> Search(string search)
        {
            search.Verify(nameof(search)).IsNotEmpty();

            //if (!_registerStore.TryGet(search, out QueueRegistration queueRegistration))
            //{
            //    return Task.FromResult<IReadOnlyList<QueueRegistration>>(Enumerable.Empty<QueueRegistration>().ToList());
            //}

            QueueRegistration queueRegistration = null;
            return Task.FromResult<IReadOnlyList<QueueRegistration>>(queueRegistration.ToEnumerable().ToList());
        }
    }
}
