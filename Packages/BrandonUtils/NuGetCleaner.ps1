<# 
    .SYNOPSIS
        Finds all of the `.nupkg` files in the `$Path` and removes any of their `.dll`s that don't target `$TargetFramework`.
    .NOTES
        This deals with multiple framework-targeting versions of the same library being unpacked into the Unity directory,
        which is (apparently) a "feature" of NuGet: https://stackoverflow.com/a/12534481
 #>
function Remove-DuplicateNugets(
        $Path,
        [ValidateNotNullOrEmpty()] $TargetFramework = "netstandard2.0"
) {
    $nugetPackages = Find-NugetPackages $Path

    foreach ($nupkg in $nugetPackages) {
        Remove-DuplicateNupkgFrameworks -nupkg $nupkg -TargetFramework $TargetFramework
    }
}

<# 
    .SYNOPSIS
        Retrieves all of the `.nupkg` files within the given `$Path`, recursively.
 #>
function Find-NugetPackages($Path) {
    Get-ChildItem -Path $Path -Recurse -Filter "*.nupkg"
}

<# 
    .SYNOPSIS
        Removes all of the .net framework folders & `.dll`s except for `$TargetFramework`.
#>
function Remove-DuplicateNupkgFrameworks(
        [System.IO.FileInfo]$nupkg,
        [ValidateNotNullOrEmpty()]$TargetFramework = "netstandard2.0"
) {
    $junk = Get-ChildItem "$nupkg/../lib" -Exclude $TargetFramework

    $junk | Remove-Item -Recurse -ErrorAction Stop

    $frameworks = Get-ChildItem "$nupkg/../lib" -Directory

    if ($frameworks.Length -ne 1) {
        throw "Expected just [$TargetFramework] after cleaning things up, but we have [$( $frameworks -join ", " )]!"
    }

    if ($frameworks[0].Name -ne $TargetFramework) {
        throw "We were left with just 1 framework, but it wasn't [$TargetFramework], it was [$( $frameworks[0] )]!"
    }
}