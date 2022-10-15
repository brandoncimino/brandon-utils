<# 
.SYNOPSIS
    A single script to invoke `Remove-DuplicateNugets` without needing to `Import-Module` first.

.DESCRIPTION
    This script triggers the functions in `Nuget.psm1`, 
    which in turn get their meat from `Nuget.cs`.
#>
param([Parameter()]$Path = $PSScriptRoot)

$ErrorActionPreference = [System.Management.Automation.ActionPreference]::Stop

Write-Host "Cleaning under: $Path"

Import-Module "$PSScriptRoot/Nuget.psm1" -Verbose -Force

[Nuget]::EnumeratePackages($Path) | Repair-Nuget -Verbose