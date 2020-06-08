using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Khooversoft.MessageNet.Interface;
using Khooversoft.Toolbox.Standard;
using Microservice.Interface;
using Microservice.Interface.Test;

namespace CustomerInfo.MicroService
{
    public class CustomerFunctions
    {
        private readonly IMessageNetSend _messageNetSend;

        public CustomerFunctions(IMessageNetSend messageNetSend)
        {
            _messageNetSend = messageNetSend;
        }

        [MessageFunction("CustomerAddress", "{namespace}/{network.id}/Get-CustomerInfo")]
        public async Task GetCustomerInfo(IWorkContext context, NetMessage netMessage, CustomerInfoRequest request)
        {
            context.VerifyNotNull(nameof(context));
            request.VerifyNotNull(nameof(request));

            var response = new CustomerInfoResponse
            {
                CustomerId = request.CustomerId,
                Addr1 = "Address 1",
                Addr2 = "Address 2",
                City = "City",
                State = "State",
                Zip = "Zip",
            };

            NetMessage reply = new NetMessageBuilder(netMessage)
                .Add(netMessage.Header.WithReply("get.response"))
                .Add(MessageContent.Create(response))
                .Build();

            await _messageNetSend.Send(context, reply);
        }

        [MessageFunction("CustomerAddress", "{namespace}/{network.id}/Post-LogCustomerAddress")]
        public async Task GetCustomerInfo(IWorkContext context, NetMessage request)
        {
            context.VerifyNotNull(nameof(context));
            request.VerifyNotNull(nameof(request));

            NetMessage reply = new NetMessageBuilder(request)
                .Add(request.Header.WithReply("get.response"))
                .Build();

            await _messageNetSend.Send(context, reply);
        }
    }
}
