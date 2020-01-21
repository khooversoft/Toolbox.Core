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
        private readonly ITestContext _testContext;

        public CustomerFunctions(ITestContext testContext)
        {
            testContext.Verify(nameof(testContext)).IsNotNull();

            _testContext = testContext;
        }

        [Function("CustomerAddress", "ms://{node.id}/{function.name}")]
        public Task GetCustomerAddress(IWorkContext context, string /*RouteMessage<CustomerInfoRequest>*/ request)
        {
            context.Verify(nameof(context)).IsNotNull();
            request.Verify(nameof(request)).IsNotNull();

            _testContext.AddMessageAsString(request);

            return Task.CompletedTask;
        }
    }
}
