using System.Collections.Generic;
using System.Linq;

namespace Khooversoft.Toolbox.Standard
{
    public class SecretFilter : ISecretFilter
    {
        private readonly HashSet<string> _secrets;

        public SecretFilter() => _secrets = new HashSet<string>();

        public SecretFilter(IEnumerable<string> secrets)
        {
            secrets.VerifyNotNull(nameof(secrets));

            _secrets = new HashSet<string>(secrets);
        }

        public string? FilterSecrets(string? data, string replaceSecretWith = "***")
        {
            if (data.IsEmpty() || _secrets.Count == 0) return data;

            replaceSecretWith.VerifyNotEmpty(nameof(replaceSecretWith));

            return _secrets.Aggregate(data!, (acc, x) => acc.Replace(x, replaceSecretWith));
        }
    }
}
