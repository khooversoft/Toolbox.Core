#
# Create service bus if does not exist
#

param (
)

$ErrorActionPreference = "Stop";

& .\Create-ResourceGroup.ps1;
& .\Create-BlobStorage.ps1;
& .\Create-ADLS.ps1;
& .\Create-ServiceBus.ps1;
