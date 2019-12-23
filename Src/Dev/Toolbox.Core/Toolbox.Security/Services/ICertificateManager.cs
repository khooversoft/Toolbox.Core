using Khooversoft.Toolbox.Standard;
using System.Collections.Generic;

namespace Khooversoft.Toolbox.Security
{
    public interface ICertificateManager
    {
        void Clear(IWorkContext context);

        void Set(IWorkContext context, LocalCertificate certificate);

        LocalCertificate? Get(IWorkContext context, string thumbprint);

        IEnumerator<LocalCertificate> LocalCertificateItems();
    }
}
