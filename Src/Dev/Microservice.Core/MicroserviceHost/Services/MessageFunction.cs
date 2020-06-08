using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Standard;
using Newtonsoft.Json;

namespace MicroserviceHost
{
    internal class MessageFunction
    {
        public MessageFunction(string name, string nodeId, IFunction function, Type messageType, ITelemetry telemetry, IWorkContext context)
        {
            name.VerifyNotEmpty(nameof(name));
            nodeId.VerifyNotEmpty(nameof(nodeId));
            function.VerifyNotNull(nameof(function));
            messageType.VerifyNotNull(nameof(messageType));
            telemetry.VerifyNotNull(nameof(telemetry));
            context.VerifyNotNull(nameof(context));

            Name = name;
            NodeId = nodeId;
            Function = function;
            MessageType = messageType;
            Telemetry = telemetry;
            Context = context;
        }

        public string Name { get; }

        public string NodeId { get; }

        public IFunction Function { get; }

        public Type MessageType { get; }

        public ITelemetry Telemetry { get; }

        public IWorkContext Context { get; }

        public async Task Receiver(NetMessage netMessage)
        {
            if (netMessage == null || netMessage.Content == null)
            {
                Telemetry.Error(Context, $"Message is null or message content is null, no message to process");
                return;
            }

            object? message = JsonConvert.DeserializeObject(netMessage.Content.Content, MessageType);
            if (message == null)
            {
                Telemetry.Error(Context, $"Message {MessageType} did not deserialize");
                return;
            }

            await Function.InjectAsync(Context, Telemetry, netMessage, message);
        }
    }
}
