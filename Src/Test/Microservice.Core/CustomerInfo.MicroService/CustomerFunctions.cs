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
        private readonly IMessageNetClient _messageNetClient;

        public CustomerFunctions(IMessageNetClient messageNetClient)
        {
            _messageNetClient = messageNetClient;
        }

        [Function("CustomerAddress", "queue://{host.id}/{function.name}")]
        public Task GetCustomerAddress(IWorkContext context, RouteMessage<CustomerInfoRequest> request)
        {
            return Task.CompletedTask;
        }
    }
}
