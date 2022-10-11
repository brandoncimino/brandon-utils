<# 
.SYNOPSIS
    A single script to invoke `Remove-DuplicateNugets` without needing to `Import-Module` first.

.DESCRIPTION
    This script triggers the functions in `Nuget.psm1`, 
    which in turn get their meat from `Nuget.cs`.
#>
Import-Module ./Nuget.psm1 -Verbose
Remove-DuplicateNugets