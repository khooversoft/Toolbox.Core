#
# Delete test service bus
#

param (
    [string] $Subscription = "Default Subscription / Directory",

    [string] $ResourceGroupName = "toolboxtesting-rg",

    [string] $NamespaceName = "messagehubtest"
)

$ErrorActionPreference = "Stop";

$currentSubscriptionName = (Get-AzContext).Subscription.Name;
if( $currentSubscriptionName -ne $Subscription )
{
    Set-AzContext -SubscriptionName $Subscription;
}

# Query to see if the namespace currently exists
$CurrentNamespace = Get-AzServiceBusNamespace -ResourceGroupName $ResourceGroupName -NamespaceName $NamespaceName -ErrorAction SilentlyContinue;

if( !$CurrentNamespace )
{
    Write-Host "The namespace $NamespaceName does not exists.";
    return;
}

Write-Host "Removing namespace $NamespaceName...";
Remove-AzServiceBusNamespace -ResourceGroupName $ResourceGroupName -Name $NamespaceName;

Write-Host "Completed";