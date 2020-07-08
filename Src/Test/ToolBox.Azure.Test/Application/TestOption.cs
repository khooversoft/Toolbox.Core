//using Khooversoft.Toolbox.Azure;
//using Microsoft.Extensions.Configuration;
//using System.IO;
//using System.Threading;
//using Khooversoft.Toolbox.Standard;

//namespace ToolBox.Azure.Test.Application
//{
//    internal class TestOption
//    {
//        internal const string _resourceId = "Toolbox.Test.Application.TestConfig.json";

//        public TestOption()
//        {
//            StoreOption option = GetBlobStoreOption();

//            DatalakeRepository = new DatalakeRepository(option)
//                .VerifyNotNull(nameof(DatalakeRepository));
//        }

//        public IDatalakeRepository DatalakeRepository { get; }

//        private StoreOption GetBlobStoreOption()
//        {
//            using Stream configStream = FileTools.GetResourceStream(typeof(TestOption), _resourceId);

//            IConfiguration configuration = new ConfigurationBuilder()
//                .AddJsonStream(configStream)
//                .AddUserSecrets("Toolbox.Test")
//                .Build();

//            var option = new StoreOption()
//                .Action(x => configuration.Bind(x, x => x.BindNonPublicProperties = true))
//                .Action(x => x.Verify());

//            return option;
//        }
//    }
//}
