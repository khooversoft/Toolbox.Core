using Khooversoft.Toolbox.Standard;
using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Khooversoft.Toolbox.Security
{
    /// <summary>
    /// Local certificate specifies X509 certificates stored in Windows certificate store.
    /// Once the certificate has been located and loaded, it is cached.
    /// </summary>
    [DebuggerDisplay("StoreName={LocalCertificateKey.StoreName}, StoreLocation={LocalCertificateKey.StoreLocation}, Thumbprint={LocalCertificateKey.Thumbprint}")]
    public class LocalCertificate
    {
        private static readonly StringVector _tag = new StringVector(nameof(LocalCertificate));
        private readonly object _lock = new object();
        private readonly CacheObject<X509Certificate2> _cachedCertificate = new CacheObject<X509Certificate2>(TimeSpan.FromDays(1));

        public LocalCertificate(LocalCertificateKey key)
        {
            key.Verify(nameof(key)).IsNotNull();

            LocalCertificateKey = key;
        }

        public LocalCertificate(StoreLocation storeLocation, StoreName storeName, string thumbprint, bool requirePrivateKey)
        {
            LocalCertificateKey = new LocalCertificateKey(storeLocation, storeName, thumbprint, requirePrivateKey);
        }

        /// <summary>
        /// Local certificate key
        /// </summary>
        public LocalCertificateKey LocalCertificateKey { get; }

        /// <summary>
        /// Find certificate by thumbprint.  Certificates that have expired will not
        /// be returned and if "throwOnNotFound" is specified, an exception will be
        /// thrown.
        /// </summary>
        /// <param name="tag">tag</param>
        /// <param name="context">work context</param>
        /// <param name="throwOnNotFound">if true, throw exception if not found</param>
        /// <exception cref="ProgramExitException">Certificate is not found</exception>
        /// <returns>X509 certificate</returns>
        /// <exception cref="CertificateNotFoundException">when certificate valid certificate was not found</exception>
        public X509Certificate2 GetCertificate(IWorkContext context, bool? throwOnNotFound = null)
        {
            context = context.With(_tag);
            X509Certificate2 certificate;

            Exception? saveException = null;
            throwOnNotFound = throwOnNotFound ?? LocalCertificateKey.RequirePrivateKey;

            lock (_lock)
            {
                if (_cachedCertificate.TryGetValue(out certificate))
                {
                    return certificate;
                }

                using (X509Store store = new X509Store(LocalCertificateKey.StoreName, LocalCertificateKey.StoreLocation))
                {
                    context.Telemetry.Verbose(context, $"Looking for certificate for {this}");

                    try
                    {
                        store.Open(OpenFlags.ReadOnly);
                        X509Certificate2Collection certificateList = store.Certificates.Find(X509FindType.FindByThumbprint, LocalCertificateKey.Thumbprint, validOnly: false);

                        if (certificateList?.Count != 0)
                        {
                            _cachedCertificate.Set(
                                certificateList
                                    .OfType<X509Certificate2>()
                                    .Where(x => !LocalCertificateKey.RequirePrivateKey || x.HasPrivateKey)
                                    .Where(x => DateTime.Now <= x.NotAfter)
                                    .FirstOrDefault()
                                );
                        }
                    }
                    catch (Exception ex)
                    {
                        context.Telemetry.Warning(context, $"Exception: {ex}");
                        _cachedCertificate.Clear();
                        saveException = ex;
                    }
                }

                context.Telemetry.Verbose(context, $"{(_cachedCertificate != null ? "Found" : "Not found")} certificate for {this}");

                if (!_cachedCertificate!.TryGetValue(out certificate) && throwOnNotFound == true)
                {
                    throw new CertificateNotFoundException($"Certificate not found: {LocalCertificateKey.ToString()}");
                }

                return certificate;
            }
        }
    }
}
