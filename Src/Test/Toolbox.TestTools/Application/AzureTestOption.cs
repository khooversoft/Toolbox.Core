using Khooversoft.Toolbox.Azure;
using Khooversoft.Toolbox.Standard;
using Microsoft.Extensions.Logging;

namespace Khoover.Toolbox.TestTools
{
    public class AzureTestOption
    {
        private IDatalakeRepository? _datalakeRepository;
        private IBlobRepository? _blobRepository;
        private IQueueManagement? _queueManagement;
        private IDatalakeManagement? _datalakeManagement;

        public BlobRepositoryOption BlobOption { get; set; } = null!;

        public DatalakeRepositoryOption DatalakeOption { get; set; } = null!;

        public QueueManagementOption QueueOption { get; set; } = null!;

        public void Verify()
        {
            BlobOption
                .VerifyNotNull(nameof(BlobOption))
                .Verify();

            DatalakeOption
                .VerifyNotNull(nameof(DatalakeOption))
                .Verify();

            QueueOption
                .VerifyNotNull(nameof(QueueOption))
                .Verify();
        }

        public IDatalakeRepository GetDatalakeRepository(ILoggerFactory loggerFactory) => _datalakeRepository
            ??= new DatalakeRepository(DatalakeOption.VerifyNotNull(nameof(DatalakeOption)), loggerFactory.CreateLogger<DatalakeRepository>());

        public IDatalakeManagement GetDatalakeManagement(ILoggerFactory loggerFactory) => _datalakeManagement
            ??= new DatalakeManagement(DatalakeOption.VerifyNotNull(nameof(DatalakeOption)), loggerFactory.CreateLogger<DatalakeManagement>());

        public IBlobRepository GetBlobRepository(ILoggerFactory loggerFactory) => _blobRepository
            ??= new BlobRepository(BlobOption.VerifyNotNull(nameof(BlobOption)), loggerFactory.CreateLogger<BlobRepository>());

        public IQueueManagement GetQueueManagement(ILoggerFactory loggerFactory) => _queueManagement
            ??= new QueueManagement(QueueOption.VerifyNotNull(nameof(QueueOption)).GetConnectionString(), loggerFactory.CreateLogger<QueueManagement>());
    }
}
