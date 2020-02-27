// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Standard;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace MessageNet.Host.Tests
{
    public class ApplicationFixture
    {
        public ApplicationFixture()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddUserSecrets("MessageNet.Host.Test")
                .Build();

            PropertyResolver = new PropertyResolver(configuration.GetChildren().ToDictionary(x => x.Key, x => x.Value));
        }

        public IPropertyResolver PropertyResolver { get; }
    }
}
