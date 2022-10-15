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

try {
    $source_file = 'Nuget.cs'
    $source_path = "$PSScriptRoot/$source_file"
    Add-Type -Path $source_path -ErrorAction Stop -Verbose
} catch {
    if ($_.Exception.Message -match "Cannot add type. The type name '.*' already exists.") {
        # for some reason, $PSStyle.Formatting.Error and ErrorAccent don't actually match what's used in the console...
        $acc = $PSStyle.Reset + $PSStyle.Foreground.Cyan
        $err = $PSStyle.Reset + $PSStyle.Foreground.BrightRed
        $link = $acc + $PSStyle.FormatHyperlink($source_file, $source_path) + $PSStyle.Reset + $err
        throw "${err}You've made a change to the source code of $link, but C# types cannot be re-loaded into an existing Powershell session. Please restart the session to import this module. ($( $PSStyle.Foreground.Blue + $PSStyle.Underline )📄 ${source_path}${err})"
    }
}
if ('Nuget' -as [type]) {
    Write-Host "⏭ The type $( $PSStyle.Foreground.Blue )[$( [Nuget] )]$( $PSStyle.Reset ) is already present ($( $PSStyle.Italic )$( [Nuget].AssemblyQualifiedName )$( $PSStyle.ItalicOff ))."
}
else {
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
        [string]$Path = $PWD) {
    foreach ($pkg in [Nuget]::EnumeratePackages()) {
        Write-Output $pkg
    }
}

<# 
.SYNOPSIS
    Cleans up a Nuget dependency for use with Unity by:
        - Only keeping 1 DLL file
        - Updating the DLL's `.meta` file with:
            - isExplicitlyReferenced    = 1
            - validateReferences        = 0

.DESCRIPTION
    Nuget will download assemblies (aka `.dll` files) for *ALL* possible target frameworks for a given package and store them inside of the `lib` folder.
    This will upset Unity, because the namespaces will conflict, and Unity doesn't understand that only one of them is being used.
    So, we use the `Repair-Nuget` command to remove these "extra" assemblies.

.NOTES
    This is the successor to `Remove-DuplicateNugets`.
#>
function Repair-Nuget(
# The path to the `.nupkg` file (or the directory that contains it)
# TODO: make this more flexible, so that it can (at least) take in a .dll file as well
        [Parameter(Mandatory, ValueFromPipeline, ValueFromPipelineByPropertyName)]
        [string]$Path,
# The .NET frameworks that we are OK with keeping, in their preferred order.
        [ValidateNotNullOrEmpty()]
        [string[]]$TargetFramework = @('netstandard2_1', 'netstandard2_0'),
# If `$true`, Unity will throw an error when the transient dependencies of a Nuget package conflict with each other.
        [bool]$ValidateReferences = $false
) {
    process {
        $nupkg_file = Get-Item $Path
        Write-Verbose "Repairing nupkg file: $nupkg_file"
        $pkg = [NugetPackage]::new($nupkg_file)

        $fw_col = {
            "$( $PSStyle.Foreground.BrightBlue )$( $Args[0] )$( $PSStyle.Reset )"
        }

        $kept_framework = $null
        foreach ($tf in $TargetFramework) {
            if ( $pkg.HasFramework($tf)) {
                $kept_framework = $tf
                Write-Host "✅ Keeping framework: $($fw_col.Invoke($tf) )"
                break
            }
        }

        if ($null -eq $kept_framework) {
            throw "The package $pkg ($( $pkg.Frameworks -join ", " )) didn't contain any of the `$TargetFrameworks ($( $TargetFramework -join ", " ))!"
        }

        foreach ($ass in $pkg.Assemblies) {
            if ( $ass.HasFramework($kept_framework)) {
                $meta_path = "$( $ass.Path ).meta"
                Write-Host "🧼 Cleaning the .meta file: $( $ass.Name )"
                $meta_yaml = Get-Content $meta_path -ErrorAction Stop | ConvertFrom-Yaml -Ordered

                $changed = @()
                $isExplicit = $meta_yaml.PluginImporter.isExplicitlyReferenced
                write-host -ForegroundColor Magenta "isExplicitlyReferenced: [$($isExplicit.GetType() )] $isExplicit"
                if ($meta_yaml.PluginImporter.isExplicitlyReferenced.Equals(1) -eq $false) {
                    $meta_yaml.PluginImporter.isExplicitlyReferenced = 1
                    $changed += "Set isExplicitlyReferenced to $( $meta_yaml.PluginImporter.isExplicitlyReferenced )"
                }

                $ValidateReferences = [int] $ValidateReferences
                if ($meta_yaml.PluginImporter.validateReferences.Equals($ValidateReferences) -eq $false) {
                    $meta_yaml.PluginImporter.validateReferences = $ValidateReferences
                    $changed += "Set validateReferences to $( $meta_yaml.PluginImporter.validateReferences )"
                }

                if ($changed) {
                    Write-Verbose (("Made changes: " + $changed) -join "`n")
                    Set-Content -Path $meta_path -Value ($meta_yaml | ConvertTo-Yaml) -Force -ErrorAction Stop

                    # make sure the stuff was ACTUALLY changed...
                    $re_meta = Get-Content $meta_path | ConvertFrom-Yaml -Ordered
                    if ($re_meta.PluginImporter.isExplicitlyReferenced -eq $false) {
                        throw "was supposed to be true!!!"
                    }
                }
                else {
                    Write-Verbose "No changes needed; leaving the .meta file alone"
                }
            }
            else {
                Write-Host "🗑 Deleting assembly: $ass"
                $ass.Delete()
            }
        }
    }
}