using Autofac;
using Khooversoft.MessageNet.Client;
using Khooversoft.MessageNet.Host;
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
        private readonly IMessageNetConfig _messageNetConfig;
        private ILifetimeScope? _lifetimeScope;
        private IReadOnlyList<FunctionMessageReceiver>? _functionMessageReceivers;

        public FunctionHost(IEnumerable<FunctionConfiguration> functionConfigurations, IMessageNetConfig messageNetConfig, ILifetimeScope? lifetimeScope = null)
        {
            functionConfigurations.VerifyNotNull(nameof(functionConfigurations));
            messageNetConfig.VerifyNotNull(nameof(messageNetConfig));

            _functionConfigurations = functionConfigurations.ToList();
            _messageNetConfig = messageNetConfig;
            _lifetimeScope = lifetimeScope;
        }

        public async Task Start(IWorkContext context)
        {
            _functionMessageReceivers = _functionConfigurations
                .Select(x => new FunctionMessageReceiver(x, _messageNetConfig, _lifetimeScope))
                .ToList();

            await _functionMessageReceivers
                .ForEachAsync(x => x.Start(context.WithActivity()));
        }

        public void Stop(IWorkContext context)
        {
            var receivers = Interlocked.Exchange(ref _functionMessageReceivers, null!);
            receivers?.ForEach(x => x.Stop(context));

            var lifetimeScope = Interlocked.Exchange(ref _lifetimeScope, null!);
            lifetimeScope?.Dispose();
        }

        public void Dispose() => Stop(WorkContextBuilder.Default);
    }
}
