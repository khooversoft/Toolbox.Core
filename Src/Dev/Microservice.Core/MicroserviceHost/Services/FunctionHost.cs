using Autofac;
using Khooversoft.MessageNet.Client;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MicroserviceHost
{
    public class FunctionHost : IDisposable
    {
        private IReadOnlyList<FunctionConfiguration> _functionConfigurations;
        private readonly IMessageNetClient _messageNetClient;
        private ILifetimeScope? _lifetimeScope;
        private IReadOnlyList<FunctionMessageReceiver>? _functionMessageReceivers;

        public FunctionHost(IEnumerable<FunctionConfiguration> functionConfigurations, IMessageNetClient messageNetClient, ILifetimeScope? lifetimeScope = null)
        {
            functionConfigurations.Verify(nameof(functionConfigurations)).IsNotNull();
            messageNetClient.Verify(nameof(messageNetClient)).IsNotNull();

            _functionConfigurations = functionConfigurations.ToList();
            _messageNetClient = messageNetClient;
            _lifetimeScope = lifetimeScope;
        }

        public async Task Start(IWorkContext context)
        {
            _functionMessageReceivers = _functionConfigurations
                .Select(x => new FunctionMessageReceiver(x, _messageNetClient, _lifetimeScope))
                .ToList();

            await _functionMessageReceivers
                .ForEachAsync(x => x.Start(context.WithNewCv()));
        }

        public void Stop(IWorkContext context)
        {
            var receivers = Interlocked.Exchange(ref _functionMessageReceivers, null!);
            receivers?.ForEach(x => x.Stop(context));

            var lifetimeScope = Interlocked.Exchange(ref _lifetimeScope, null!);
            lifetimeScope?.Dispose();
        }

        public void Dispose()
        {
            Stop(WorkContext.Empty);
        }
    }
}
