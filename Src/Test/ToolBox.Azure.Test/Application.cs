using Khooversoft.Toolbox.Standard;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using Xunit;

namespace ToolBox.Azure.Test
{
    public class Application
    {
        public Application()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddUserSecrets("ToolBox.Azure.Test")
                .Build();

            PropertyResolver = new PropertyResolver(configuration.GetChildren().ToDictionary(x => x.Key, x => x.Value));
        }

        public IPropertyResolver PropertyResolver { get; }
    }
}
