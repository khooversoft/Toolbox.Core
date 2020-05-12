using Khooversoft.MessageNet.Host;
using Khooversoft.Toolbox.Standard;
using MicroserviceHost;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CustomerInfo.Microservice.Test.Application
{
    public class ApplicationFixture
    {
        internal const string _resourceId = "CustomerInfo.Microservice.Test.Application.TestConfig.json";

        public ApplicationFixture()
        {
            using Stream configStream = FileTools.GetResourceStream(typeof(ApplicationFixture), _resourceId);

            Option = new OptionBuilder()
                .SetUserSecretId("CustomerInfo.Microservice.Test")
                .Build();
        }

        public IOption Option { get; }
    }
}
