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
            testContext.VerifyNotNull(nameof(testContext));

            _testContext = testContext;
        }

        [MessageFunction("CustomerAddress", "test/Get-CustomerInfo")]
        public Task GetCustomerInfo(IWorkContext context, CustomerInfoRequest request)
        {
            context.VerifyNotNull(nameof(context));
            request.VerifyNotNull(nameof(request));

            _testContext.AddMessage(request);

            return Task.CompletedTask;
        }

        [MessageFunction("CustomerAddress", "test/Post-LogCustomerAddress")]
        public Task GetCustomerInfo(IWorkContext context, NetMessage request)
        {
            context.VerifyNotNull(nameof(context));
            request.VerifyNotNull(nameof(request));

            _testContext.AddMessage(request);

            return Task.CompletedTask;
        }
    }
}
