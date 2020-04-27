using Khooversoft.MessageNet.Host;
using Khooversoft.Toolbox.Standard;
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
        private IMessageNetConfig _config;

        public ApplicationFixture()
        {
            using Stream configStream = FileTools.GetResourceStream(typeof(ApplicationFixture), _resourceId);

            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonStream(configStream)
                .AddUserSecrets("CustomerInfo.Microservice.Test")
                .Build();

            var option = new TestOption()
                .Action(x => configuration.Bind(x, x => x.BindNonPublicProperties = true))
                .Action(x => x.Verify());

            _config = new MessageNetConfig(new NamespaceRegistration("default", option.GetConnectionString()));
        }

        public IMessageNetConfig GetMessageNetConfig() => _config;
    }
}
