using Khooversoft.Toolbox.Standard;

namespace CustomerInfo.Microservice.Test.Application
{
    internal class TestOption
    {
        public string? NamespaceName { get; set; }

        public string? SharedAccessKeyName { get; set; }

        public string? SharedAccessKey { get; set; }

        public void Verify()
        {
            NamespaceName.VerifyNotEmpty(nameof(NamespaceName));
            SharedAccessKeyName.VerifyNotEmpty(nameof(SharedAccessKeyName));
            SharedAccessKey.VerifyNotEmpty(nameof(SharedAccessKey));
        }

        public string GetConnectionString() =>
            $"Endpoint=sb://{NamespaceName}.servicebus.windows.net/;SharedAccessKeyName={SharedAccessKeyName};SharedAccessKey={SharedAccessKey};TransportType=Amqp";
    }
}
