using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Toolbox.Azure
{
    public class QueueManagementOption
    {
        public string? ConnectionString { get; set; }

        public string? SharedAccessKey { get; set; }

        public void Verify()
        {
            ConnectionString.VerifyNotEmpty(nameof(ConnectionString));
            SharedAccessKey.VerifyNotEmpty(nameof(SharedAccessKey));
        }

        public string GetConnectionString()
        {
            ConnectionString.VerifyNotEmpty(nameof(ConnectionString));
            SharedAccessKey.VerifyNotEmpty(nameof(SharedAccessKey));

            var properties = new[]
            {
                new KeyValuePair<string, string>(nameof(SharedAccessKey), SharedAccessKey),
            };

            return new PropertyResolver(properties).Resolve(ConnectionString)!;
        }
    }
}
