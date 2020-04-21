// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.MessageNet.Host;
using Khooversoft.Toolbox.Standard;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Linq;

namespace MessageNet.Host.Tests.Application
{
    public class ApplicationFixture
    {
        internal const string _resourceId = "MessageNet.Host.Tests.Application.TestConfig.json";
        private IMessageNetConfig _config;

        public ApplicationFixture()
        {
            using Stream configStream = FileTools.GetResourceStream(typeof(ApplicationFixture), _resourceId);

            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonStream(configStream)
                .AddUserSecrets("MessageNet.Host.Test")
                .Build();

            var option = new TestOption()
                .Action(x => configuration.Bind(x, x => x.BindNonPublicProperties = true))
                .Action(x => x.Verify());

            _config = new MessageNetConfig(new NamespaceRegistration("default", option.GetConnectionString()));
        }

        public IMessageNetConfig GetMessageNetConfig() => _config;
    }
}
