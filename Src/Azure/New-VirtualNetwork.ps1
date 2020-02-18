<#
.DESCRIPTION
Create a new virtual network if it does not exist.

.PARAMETER






#>


Param(
    [Parameter(Mandatory)]
    [string] $Subscription,

    [Parameter(Mandatory)]
    [string] $ResourceGroup,

    [Parameter(Mandatory)]
    [string] $VirtualNetworkName,

    [Parameter(Mandatory)]
    [string] $Location,

    [Parameter(Mandatory)]
    [string] $Name,

    [Parameter(Mandatory)]
    [string] $AddressPrefix = "10.0.0.0/16",

    [Parameter(Mandatory)]
    [hashtable] $Subnet
)

# Lookup virtual net
