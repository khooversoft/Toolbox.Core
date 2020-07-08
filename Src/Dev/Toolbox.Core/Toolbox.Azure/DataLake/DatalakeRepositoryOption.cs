using Khooversoft.Toolbox.Standard;
using System.Collections.Generic;

namespace Khooversoft.Toolbox.Azure
{
    public class DatalakeRepositoryOption
    {
        public string AccountName { get; set; } = null!;

        public string AccountKey { get; set; } = null!;

        public string FileSystemName { get; set; } = null!;

        public void Verify()
        {
            AccountName.VerifyNotEmpty(nameof(AccountName));
            AccountKey.VerifyNotEmpty(nameof(AccountKey));
            FileSystemName.VerifyNotEmpty(nameof(FileSystemName));
        }
    }
}
