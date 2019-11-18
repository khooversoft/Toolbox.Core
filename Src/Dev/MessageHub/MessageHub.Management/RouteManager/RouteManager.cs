using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
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
                .Add(new CreateQueueState(_serviceBusConnection, queueRegistration))
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
    }
}
