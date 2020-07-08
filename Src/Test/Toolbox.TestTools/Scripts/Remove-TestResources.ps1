#
# Remove resource group
#

param (
    [string] $Subscription = "Default Subscription / Directory",

    [string] $Name = "toolboxtesting-rg"
)

$ErrorActionPreference = "Stop";

Write-Host "Switching to subscription $Subscription";

$currentSubscriptionName = (Get-AzContext).Subscription.Name;
if( $currentSubscriptionName -ne $Subscription )
{
    Set-AzContext -SubscriptionName $Subscription;
}

$resourceGroup = Get-AzResourceGroup -Name $Name -ErrorAction SilentlyContinue;

if( !$resourceGroup )
{
    Write-Error "$Name does not exist";
    Exit(1);
}

Write-Host "Removing resource group $Name";

Remove-AzResourceGroup -Name $Name -Force;
Write-Host "$Name resouce group has been removed";
