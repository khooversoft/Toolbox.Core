using System.Collections.Generic;
using System.Linq;

namespace Khooversoft.Toolbox.Standard
{
    /// <summary>
    /// Secret filter, used to mask out secrets in text.  Normal used to filter out secrets in logging.
    /// </summary>
    public class SecretFilter : ISecretFilter
    {
        private readonly HashSet<string> _secrets;

        /// <summary>
        /// Create empty secret collection
        /// </summary>
        public SecretFilter() => _secrets = new HashSet<string>();

        /// <summary>
        /// Create a secret collection with secrets.  Secrets are case sensitive.
        /// </summary>
        /// <param name="secrets"></param>
        public SecretFilter(IEnumerable<string> secrets)
        {
            secrets.VerifyNotNull(nameof(secrets));

            _secrets = new HashSet<string>(secrets);
        }

        /// <summary>
        /// Filter out secrets in string.
        /// </summary>
        /// <param name="data">source string</param>
        /// <param name="replaceSecretWith">replace secrets with</param>
        /// <returns>processed data</returns>
        public string? FilterSecrets(string? data, string replaceSecretWith = "***")
        {
            replaceSecretWith = replaceSecretWith ?? string.Empty;
            
            if (data.IsEmpty() || _secrets.Count == 0) return data;

            return _secrets.Aggregate(data!, (acc, x) => acc.Replace(x, replaceSecretWith));
        }
    }
}
