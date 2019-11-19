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
        private readonly ServiceBusConnection _serviceBusConnection;
        private readonly IRegisterStore _registerStore;

        public RouteManager(ServiceBusConnection serviceBusConnection, IRegisterStore registerStore)
        {
            serviceBusConnection.Verify(nameof(serviceBusConnection)).IsNotNull();
            registerStore.Verify(nameof(registerStore)).IsNotNull();

            _serviceBusConnection = serviceBusConnection;
            _registerStore = registerStore;
        }

        public async Task<bool> Register(IWorkContext context, QueueRegistration queueRegistration)
        {
            queueRegistration.Verify(nameof(queueRegistration)).IsNotNull();

            _registerStore.Set(context, queueRegistration);

            return await new StateManagerBuilder()
                .Add(new CreateQueueState(_serviceBusConnection, queueRegistration.QueueDefinition))
                .Build()
                .Set(context);
        }

        public async Task<bool> Unregister(IWorkContext context, string nodeId)
        {
            nodeId.Verify(nameof(nodeId)).IsNotNull();

            _registerStore.Remove(context, nodeId);

            return await new StateManagerBuilder()
                .Add(new RemoveQueueState(_serviceBusConnection, nodeId))
                .Build()
                .Set(context);
        }

        public Task<IReadOnlyList<QueueRegistration>> Search(string search)
        {
            search.Verify(nameof(search)).IsNotEmpty();

            if (!_registerStore.TryGet(search, out QueueRegistration queueRegistration))
            {
                return Task.FromResult<IReadOnlyList<QueueRegistration>>(Enumerable.Empty<QueueRegistration>().ToList());
            }

            return Task.FromResult<IReadOnlyList<QueueRegistration>>(queueRegistration.ToEnumerable().ToList());
        }
    }
}
