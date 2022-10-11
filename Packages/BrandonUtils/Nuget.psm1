<#
.SYNOPSIS
    Contains commands to manipulate Nuget packages and assemblies.

.DESCRIPTION
    The meat of this module is controlled by the classes in `Nuget.cs`:
        - `[Nuget]`, which contains static utility methods
        - `[NugetPackage]`, which wraps a `.nupkg` file
        - `[NugetAssembly]`, which wraps a `.dll` file

TODO: Combine this, `BuildAutomator.ps1`, etc. into a single `UnityDev.psd1` module.

#>

Add-Type -Path "$PSScriptRoot/Nuget.cs" -ErrorAction Stop -Verbose
if('Nuget' -as [type]){
    Write-Host "⏭ The type $($PSStyle.Foreground.Blue)[$([Nuget])]$($PSStyle.Reset) is already present ($($PSStyle.Italic)$([Nuget].AssemblyQualifiedName)$($PSStyle.ItalicOff))."
}
else{
    Write-Host "💾 Importing Nuget.cs"
    Add-Type -Path "$PSScriptRoot/Nuget.cs" -ErrorAction Stop -Verbose
}

<#
.SYNOPSIS
    Retrieves all of the `.nupkg` files inside of the current directory as the fancy `[NugetPackage]` type.

.OUTPUTS
    NugetPackage
#>
function Get-NugetPackages(
    # The parent path to search inside for `.nupkg` files.
    [Parameter(
        ValueFromPipeline,
        ValueFromPipelineByPropertyName
        )]
[string]$Path = $PWD){
    foreach($pkg in [Nuget]::EnumeratePackages()){
        Write-Output $pkg
    }
}

<# 
.SYNOPSIS
    Searches for installations of a Nuget package that aren't `$TargetFramework` and removes them.

.DESCRIPTION
    Nuget will download assemblies (aka `.dll` files) for *ALL* possible target frameworks for a given package and store them inside of the `lib` folder.
    This will upset Unity, because the namespaces will conflict, and Unity doesn't understand that only one of them is being used.
    So, we use the `Remove-DuplicateNugets` command to remove this "extra" assemblies.
 #>
function Remove-DuplicateNugets(
    [ValidateNotNullOrEmpty()]
    [Parameter()]
    $TargetFramework = "netstandard2.0"
){
    [NugetPackage[]]$packages = Get-NugetPackages

    foreach($pkg in $packages){
        Write-Verbose "Checking $pkg for duplicate frameworks..."
        if($pkg.Frameworks.Contains($TargetFramework) -eq $false){
            throw "The Nuget package $pkg doesn't have the framework $TargetFramework installed!"
        }

        foreach($ass in $pkg.Assemblies){
            if($ass.Framework -ne $TargetFramework){
                Write-Host "Found duplicate framework: $ass; deleting it..."
                $ass.Delete();
            }
        }

        Write-Verbose "Verifying that ONLY $TargetFramework remains for $pkg..."

        $assAfter = $pkg.Assemblies
        if($assAfter.Length -ne 1 -or $assAfter[0].Framework -ne $TargetFramework){
            throw "Exepcted ONLY $TargetFramework to remain for $pkg, but found $assAfter!"
        }

        Write-Host "🧼 $($pkg.Colorize(
            $PSStyle.Bold + $PSStyle.Foreground.Cyan,
            $PSStyle.Italic + $PSStyle.Foreground.BrightBlack,
            $PSStyle.Reset + $PSStyle.Foreground.Green
        )) is clean."
    }
}