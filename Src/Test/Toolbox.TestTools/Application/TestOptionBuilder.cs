using Khooversoft.Toolbox.Standard;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IO;

namespace Khoover.Toolbox.TestTools
{
    public class TestOptionBuilder
    {
        public TestOptionBuilder() { }

        public AzureTestOption Build()
        {
            using Stream configStream = FileTools.GetResourceStream(typeof(TestOptionBuilder), ResourceId);

            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonStream(configStream)
                .AddUserSecrets("Toolbox.Test")
                .Build();

            var option = new AzureTestOption();
            configuration.Bind(option, x => x.BindNonPublicProperties = true);
            option.Verify();

            return option;
        }

        public static string ResourceId { get; } = "Khoover.Toolbox.TestTools.TestConfig.AzureTest.json";
    }
}
