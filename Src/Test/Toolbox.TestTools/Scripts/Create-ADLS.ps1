#
# Create ADLS Storage
#

param (
    [string] $Subscription = "Default Subscription / Directory",

    [string] $ResourceGroupName = "toolboxtesting-rg",

    [string] $AdlsName = "toolboxtestadls",

    [string] $FileSystemName = "test-adls",

    [string] $Location = "westus2"
)

$ErrorActionPreference = "Stop";

$currentSubscriptionName = (Get-AzContext).Subscription.Name;
if( $currentSubscriptionName -ne $Subscription )
{
    Set-AzContext -SubscriptionName $Subscription;
}

New-AzStorageAccount -ResourceGroupName $ResourceGroupName `
    -Name $AdlsName `
    -Location $Location `
    -SkuName Standard_RAGRS `
    -Kind StorageV2 `
    -EnableHierarchicalNamespace $true;

Write-Host "$AdlsName has been created";
