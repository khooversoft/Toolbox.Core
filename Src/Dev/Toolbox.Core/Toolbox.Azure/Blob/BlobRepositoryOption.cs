using Khooversoft.Toolbox.Standard;
using System.Collections.Generic;

namespace Khooversoft.Toolbox.Azure
{
    public class BlobRepositoryOption
    {
        public string ContainerName { get; set; } = null!;

        public string ConnectionString { get; set; } = null!;

        public string AccountKey { get; set; } = null!;

        public BlobRepositoryOption WithContainer(string container)
        {
            container.VerifyNotEmpty(nameof(container));

            return new BlobRepositoryOption
            {
                ContainerName = container,
                ConnectionString = this.ConnectionString,
                AccountKey = this.AccountKey,
            };
        }

        public void Verify()
        {
            ContainerName.VerifyNotEmpty(nameof(ContainerName));
            ConnectionString.VerifyNotEmpty(nameof(ConnectionString));
            AccountKey.VerifyNotEmpty(nameof(AccountKey));
        }

        public string GetResolvedConnectionString()
        {
            AccountKey.VerifyNotEmpty(nameof(AccountKey));
            ConnectionString.VerifyNotEmpty(nameof(ConnectionString));

            var properties = new[]
            {
                new KeyValuePair<string, string>(nameof(AccountKey), AccountKey),
            };

            return new PropertyResolver(properties).Resolve(ConnectionString)!;
        }
    }
}
