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
        [Function("CustomerAddress", "queue://{node.id}/{function.name}")]
        public Task GetCustomerAddress(IWorkContext context, RouteMessage<CustomerInfoRequest> request)
        {
            return Task.CompletedTask;
        }
    }
}
