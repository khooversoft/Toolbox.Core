#
# Create ADLS Storage
#

param (
    [string] $Subscription = "Default Subscription / Directory",

    [string] $ResourceGroupName = "toolboxtesting-rg",

    [string] $Location = "westus2"
)

$ErrorActionPreference = "Stop";

$currentSubscriptionName = (Get-AzContext).Subscription.Name;
if( $currentSubscriptionName -ne $Subscription )
{
    Set-AzContext -SubscriptionName $Subscription;
}

$currentResource = Get-AzResourceGroup -Name $ResourceGroupName -ErrorAction SilentlyContinue;
if( $currentResource )
{
    Write-Host $"Resouce group $ResourceGroupName already exist";
    return;
}

New-AzResourceGroup -Name $ResourceGroupName -Location $Location;
Write-Host "Resource group $ResourceGroupName created";
