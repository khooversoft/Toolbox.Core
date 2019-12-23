using Khooversoft.Toolbox.Standard;
using System;
using System.Security.Cryptography.X509Certificates;

namespace Khooversoft.Toolbox.Security
{
    public static class X509Certificate2Extensions
    {
        public static bool IsExpired(this X509Certificate2 self)
        {
            self.Verify(nameof(self)).IsNotNull();

            return DateTime.Now > self.NotAfter;
        }
    }
}
