using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolbox.Core.Extensions.Configuration
{
    public class GetSecretsFromKeyVault : IOptionGetSecrets
    {
        /// <summary>
        /// Get key vault secrets from key vault configuration
        /// </summary>
        /// <param name="keyVaultConfiguration">key vault configuration, if null will return null</param>
        /// <returns>dictionary of key + secret</returns>
        public IReadOnlyDictionary<string, string> GetSecrets(KeyVaultConfiguration keyVaultConfiguration)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .AddAzureKeyVault(
                    vault: $"https://{keyVaultConfiguration.KeyVaultName}.vault.azure.net/",
                    clientId: keyVaultConfiguration.AadClientId,
                    clientSecret: keyVaultConfiguration.AadClientSecret);

            IConfiguration configuration = builder.Build();

            var secrets = keyVaultConfiguration.Keys
                .ToDictionary(x => x.Value, x => configuration[x.Key]);

            return secrets;
        }
    }
}
