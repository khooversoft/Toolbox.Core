using Khooversoft.Toolbox.Configuration;
using Khooversoft.Toolbox.Standard;

namespace MessageHub.NameServer
{
    public class BlobRepositoryDetails
    {
        public string ContainerName { get; set; } = null!;

        public string Connection { get; set; } = null!;

        public void Verify()
        {
            ContainerName!.Verify(nameof(ContainerName)).IsNotEmpty();
            Connection!.Verify(nameof(Connection)).IsNotEmpty();
        }
    }
}
