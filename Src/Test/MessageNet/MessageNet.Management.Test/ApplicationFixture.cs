using Khooversoft.Toolbox.Standard;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessageNet.Management.Test
{
    public class ApplicationFixture
    {
        public ApplicationFixture()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddUserSecrets("MessageNet.Management.Test")
                .Build();

            PropertyResolver = new PropertyResolver(configuration.GetChildren().ToDictionary(x => x.Key, x => x.Value));
        }

        public IPropertyResolver PropertyResolver { get; }
    }
}
