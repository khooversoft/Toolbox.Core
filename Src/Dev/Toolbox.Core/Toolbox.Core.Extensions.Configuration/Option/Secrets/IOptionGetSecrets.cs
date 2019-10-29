using System;
using System.Collections.Generic;
using System.Text;

namespace Toolbox.Core.Extensions.Configuration
{
    public interface IOptionGetSecrets
    {
        IReadOnlyDictionary<string, string> GetSecrets(KeyVaultConfiguration keyVaultConfiguration);
    }
}
