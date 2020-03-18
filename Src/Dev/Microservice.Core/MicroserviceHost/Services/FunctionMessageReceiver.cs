//using Autofac;
//using Khooversoft.MessageNet.Client;
//using Khooversoft.Toolbox.Standard;
//using Microsoft.Azure.ServiceBus;
//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

//namespace MicroserviceHost
//{
//    /// <summary>
//    /// Function message receiver
//    /// 
//    /// This class is responsible for starting, stopping, and calling function method when message is received
//    /// 
//    /// </summary>
//    public class FunctionMessageReceiver : IDisposable
//    {
//        private readonly FunctionConfiguration _functionConfiguration;
//        private readonly ILifetimeScope? _lifetimeScope;
//        private IWorkContext? _workContext;
//        private bool _running = false;
//        public object? _functionClassInstance;
//        public IMessageNetClient _messageNetClient;

//        public FunctionMessageReceiver(FunctionConfiguration functionConfiguration, IMessageNetClient messageNetClient, ILifetimeScope? lifetimeScope = null)
//        {
//            functionConfiguration.Verify(nameof(functionConfiguration)).IsNotNull();
//            messageNetClient.Verify(nameof(messageNetClient)).IsNotNull();

//            _functionConfiguration = functionConfiguration;
//            _messageNetClient = messageNetClient;
//            _lifetimeScope = lifetimeScope;
//        }

//        /// <summary>
//        /// Start receiver.
//        /// 
//        ///   Create class for function, new instance per function
//        ///   
//        ///   Register the receiver
//        /// </summary>
//        /// <param name="context">context</param>
//        /// <returns>task</returns>

//        public async Task Start(IWorkContext context)
//        {
//            _running.Verify().Assert(x => x == false, "Receiver is already running");

//            _workContext = context;

//            context.Telemetry.Info(context, $"Create function for {_functionConfiguration.NodeId}");

//            _functionClassInstance = (_lifetimeScope?.IsRegistered(_functionConfiguration.Function.MethodInfo.DeclaringType!) == true ? _lifetimeScope.Resolve(_functionConfiguration.Function.MethodInfo.DeclaringType!) : null) ??
//                context.Container?.GetService(_functionConfiguration.Function.MethodInfo.DeclaringType!) ??
//                Activator.CreateInstance(_functionConfiguration.Function.MethodInfo.DeclaringType!);

//            context.Telemetry.Info(context, $"Starting receiver for {_functionConfiguration.NodeId}");
//            await _messageNetClient!.RegisterReceiver(context, _functionConfiguration.NodeId!, x => ProcessorMessage(x));
//        }

//        /// <summary>
//        /// Stop receiver
//        /// </summary>
//        /// <param name="context"></param>
//        /// <returns>task</returns>
//        public void Stop(IWorkContext context)
//        {
//            context.Verify(nameof(context)).IsNotNull();
//            _running.Verify().Assert(x => x == true, "Receiver is not running");

//            context.Telemetry.Info(context, $"Stopping receiver for {_functionConfiguration.NodeId}");
//            IMessageNetClient? subject = Interlocked.Exchange(ref _messageNetClient, null!);
//            subject?.Dispose();
//        }

//        /// <summary>
//        /// Process message received
//        /// 
//        ///   If string, just return
//        ///   If other type, use JSON deserialize to construct message
//        ///   
//        /// </summary>
//        /// <param name="message">message received</param>
//        /// <returns>Task</returns>
//        private Task ProcessorMessage(Message message)
//        {
//            if (_workContext!.CancellationToken.IsCancellationRequested) return Task.CompletedTask;

//            message.Verify(nameof(message))
//                .IsNotNull()
//                .Assert(x => x.Size > 0, "Message is null or size is 0");

//            string json = Encoding.UTF8.GetString(message.Body);
//            object functionMessage;

//            switch(_functionConfiguration.Function.MessageType)
//            {
//                case Type stringType when stringType == typeof(string):
//                    functionMessage = json;
//                    break;

//                default:
//                    functionMessage = JsonConvert.DeserializeObject(json, _functionConfiguration.Function.MessageType)!;
//                    functionMessage.Verify().IsNotNull("Message is null after deserialize");
//                    break;
//            }

//            try
//            {
//                _workContext.Telemetry.Verbose(_workContext, $"Invoking function for node id {_functionConfiguration.NodeId}");

//                _functionConfiguration.Function.MethodInfo.Invoke(_functionClassInstance, new object[]
//                {
//                    _workContext,
//                    functionMessage
//                });
//            }
//            catch(Exception ex)
//            {
//                _workContext.Telemetry.Error(_workContext, $"Calling function for node id {_functionConfiguration.NodeId}", ex);
//                throw;
//            }

//            return Task.CompletedTask;
//        }

//        public void Dispose()
//        {
//            IMessageNetClient? subject = Interlocked.Exchange(ref _messageNetClient, null!);
//            subject?.Dispose();
//        }
//    }
//}
