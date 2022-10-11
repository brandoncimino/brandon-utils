<# 
    https://docs.unity3d.com/Manual/CommandLineArguments.html

    .SYNOPSIS
    Butler is downloaded to & expanded in $butler_dir

    Uses the UnitySetup module: https://github.com/microsoft/unitysetup.powershell
 
    TODO: Combine this with `Nuget.psm1`, etc. into a single `UnityDev.psd1` module 
 
 # >


function Find-ParentFolder($Path, $ParentName) {
    if (Split-Path $Path -Leaf -eq $ParentName) {
        return $Path
    }

    return Find-ParentFolder ($Path | Split-Path -Parent) $ParentName
}

function Find-UnityPackageRoot($Path = $PWD) {
    if (!$Path) {
        return $null
    }
    
    try {
        return Join-Path $Path ".." "package.json" -Resolve | Split-Path -Parent
    }
    catch {
        return Find-UnityPackageRoot ($Path | Split-Path -Parent)
    }
}

function Find-UnityProjectRoot($Path = $PWD) {
    $has_assets_folder = Test-Path (Join-Path $Path "Assets")
    $has_library_folder = Test-Path(Join-Path $Path "Library")
    $has_packages_folder = Test-Path(Join-Path $Path "Packages")

    if ($has_assets_folder -and $has_library_folder -and $has_packages_folder) {
        return $Path
    }

    return Find-UnityProjectRoot ($Path | Split-Path -Parent)
}

# Exit if there is any error in the build script
$ErrorActionPreference = "Stop"

<#
.SYNOPSIS
    Creates the enum `[OS]` (if it doesn't already exist).
#>
function Import-OsEnum {
    #region Enums
    if ($null -eq ('OS' -as [type])) {
        Write-Verbose "Enum [OS] does not exist; creating it..."

        enum OS {
            Windows
            Mac
            Linux
        }

        Update-TypeData -TypeName [OS] -MemberName Current -MemberType ScriptProperty -Value {
            if ($IsWindows) {
                return [OS]::Windows
            }
            elseif ($IsMacOS) {
                return [OS]::Mac
            }
            elseif ($IsLinux) {
                return [OS]::Linux
            }
            else {
                throw "Unity projects cannot be built from a Blackberry."
            }
        }
    }
    else {
        Write-Information "⏭ The `[OS]` enum already exists."
    }
}

Import-OsEnum

enum UnityBuildTarget {
    WebGL
}
#endregion

#tag::install-butler[]
<# 
.SYNOPSIS
    Downloads and extracts Butler, Itch.io's build automation tool.
 #>
function Install-Butler() {
    #region butler constants
    $butler_name = "butler"
    $butler_dir = Join-Path (Find-UnityProjectRoot) $butler_name
    $butler_base_url = "https://broth.itch.ovh/butler"
    $butler_release_channels = @{
        [OS]::Windows = "windows-amd64"
        [OS]::Mac     = "darwin-amd64"
        [OS]::Linux   = "linux-amd64"
    }
    #endregion 

    Write-Host -ForegroundColor DarkGray "Installing butler to 📁 $butler_dir"

    #region Clean the butler dir
    Write-Host -ForegroundColor DarkGray "Cleaning $butler_dir"
    Remove-Item -Path $butler_dir -Force -ErrorAction Ignore -Recurse
    New-Item -Path $butler_dir -ItemType Directory -Force
    #endregion

    #region Download the .zip file
    $butler_release_channel = $butler_release_channels[[OS]::Current]
    $butler_url = "$butler_base_url/$butler_release_channel/LATEST/archive/default"
    $out_file_name = "butler.zip"
    $out_file_path = Join-Path $butler_dir $out_file_name

    Write-Host -ForegroundColor DarkGray "Downloading butler from [$butler_url]"

    Invoke-WebRequest -Uri $butler_url -OutFile $out_file_path

    $butler_zip = Get-Item $out_file_path -ErrorAction Stop

    Write-Host -ForegroundColor Green "Downloaded butler to [$butler_zip]"
    #endregion

    #region Extract the .zip file
    Write-Host -ForegroundColor DarkGray "Extracting butler..."

    Expand-Archive $butler_zip -DestinationPath "$butler_dir"

    $butler_exe = Get-Item "$butler_dir/butler.exe"

    Write-Host -ForegroundColor Green "Extracted butler: $butler_exe"
    #endregion

    return $butler_exe
}
#end::install-butler[]

function Get-UnityCredentials() {
    try {
        $creds_file_name = "unity_credentials.json"

        $creds_file_path = Join-Path (Find-UnityProjectRoot) ".." $creds_file_name
        
        $creds_obj = Get-Content $creds_file_path | ConvertFrom-Json
        
        if (!$creds_obj.password -or !$creds_obj.username) {
            throw [System.MissingFieldException]::new("You must provide both a password and a username in [$creds_file_name]! Actual content: $($creds_obj | Out-String)", $_.exception)
        }

        return $creds_obj
    }
    catch {
        throw [System.UnauthorizedAccessException]::new("Unable to retrieve Unity credentials from [$creds_file_name], which should be located *next to the project root folder*, at [$creds_file_path]", $_.Exception)
    }

    # I'm so tired...but I just can't bring myself to delete this...because it lines up so nicely and the colors are so nice...
    # $cred_fallbacks = [ordered]@{
    #     package = Get-UnityCredentialsFromFile (Join-Path (Find-UnityPackageRoot) $creds_file_name)
    #     project = Get-UnityCredentialsFromFile (Join-Path (Find-UnityProjectRoot) $creds_file_name)
    #     sibling = Get-UnityCredentialsFromFile (Join-Path (Find-UnityProjectRoot) ".." $creds_file_name)
    # }
}

<# 
    See: https://docs.unity3d.com/Manual/CommandLineArguments.html

    Start-UnityEditor -ExecuteMethod Build.Invoke -BatchMode -Quit -LogFile .\build.log -Wait -AdditionalArguments "-BuildArg1 -BuildArg2"
 #>
function Build-Unity() {
    # check if UnitySetup is installed
    # TODO: Why was I doing it this way, instead of using #Requires? (https://devblogs.microsoft.com/scripting/powertip-require-specific-module-in-powershell-script/)
    if (!(Get-Module UnitySetup -ListAvailable)) {
        Write-Host -ForegroundColor DarkGray "Installing the dependency, UnitySetup."
        Install-Module UnitySetup -Scope CurrentUser -AllowClobber -Force -Verbose
    }
    else {
        Write-Host -ForegroundColor DarkGray "UnitySetup was already installed."
    }

    # exit if we aren't actually in a Unity project
    $unity_project_folder = Find-UnityPackageRoot
    $unity_project = Get-UnityProjectInstance $unity_project_folder

    if (!$unity_project) {
        throw "The current directory is not inside of a Unity project! `$PWD: $PWD"
    }

    Write-Host -ForegroundColor DarkGray "Discovered Unity project:`n$( $unity_project | Format-List | Out-String )"

    Start-UnityEditor -Project $unity_project -ExecuteMethod "BrandonUtils.Editor.CommandLine.BuildAutomator.WebGL" -BatchMode -Quit -Wait -LogFile Logs/build.txt
    Write-Host -ForegroundColor Green "Succesfully built Unity! WOOH! 🎊"
}

function Compress-Unity() {
    Write-Error "TODO: compress the webgl folder into a .zip file"
}

function Deploy-Unity() {
    Write-Error "TODO: Upload the `Compress-Unity` result via Butler"
}

