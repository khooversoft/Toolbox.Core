#
# Create service bus if does not exist
#

param (
    [string] $Subscription = "Default Subscription / Directory",

    [string] $ResourceGroupName = "toolboxtesting-rg",

    [string] $StorageAccountName = "toolboxteststorage",

    [string] $Location = "westus2"
)

$ErrorActionPreference = "Stop";

$currentSubscriptionName = (Get-AzContext).Subscription.Name;
if( $currentSubscriptionName -ne $Subscription )
{
    Set-AzContext -SubscriptionName $Subscription;
}

New-AzStorageAccount -ResourceGroupName $ResourceGroupName `
    -Name $StorageAccountName `
    -Location $Location `
    -SkuName Standard_RAGRS `
    -Kind StorageV2;

Write-Host "$StorageAccountName has been created";
