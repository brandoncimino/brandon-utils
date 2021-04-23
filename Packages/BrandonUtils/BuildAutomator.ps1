<# 
    https://docs.unity3d.com/Manual/CommandLineArguments.html

    .SYNOPSIS
    Butler is downloaded to & expanded in $butler_dir
 #>
function Find-ParentFolder($Path, $ParentName) {
    if (Split-Path $Path -Leaf -eq $ParentName) {
        return $Path
    }

    return Find-ParentFolder ($Path | Split-Path -Parent) $ParentName
}

function Find-UnityPackageRoot($Path = $PWD) {
    if (!$Path) {
        Write-Host -ForegroundColor darkmagenta "path [$path] aint 'gun work"
        return $null
    }
    Write-Host -ForegroundColor magenta "checking if $Path or its parent is a the package root"
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

#region Enums
enum OS {
    Windows
    Mac
    Linux
}
function Get-OS() {
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
        throw "You seem to not _have_ an operating system. `$env:OS = $env:OS"
    }
}

enum UnityBuildTarget {
    WebGL
}
#endregion


#tag::install-butler[]
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

    Write-Host -ForegroundColor DarkGray "Installing butler to $butler_dir"

    #region Clean the butler dir
    Write-Host -ForegroundColor DarkGray "Cleaning $butler_dir"
    Remove-Item -Path $butler_dir -Force -ErrorAction Ignore -Recurse
    New-Item -Path $butler_dir -ItemType Directory -Force
    #endregion

    #region Download the .zip file
    $butler_release_channel = $butler_release_channels[(Get-Os)]
    $butler_url = "$butler_base_url/$butler_release_channel/LATEST/archive/default"
    $out_file_name = "butler.zip"
    $out_file_path = Join-Path $butler_dir $out_file_name

    Write-Host -ForegroundColor DarkGray "Downloading butler from [$butler_url]"

    Invoke-WebRequest -Uri $butler_url -OutFile $out_file_path

    $butler_zip = Get-Item $out_file_path

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
        
        if(!$creds_obj.password -or !$creds_obj.username){
            throw [System.MissingFieldException]::new("You must provide both a password and a username in [$creds_file_name]! Actual content: $($creds_obj | out-string)",$_.exception)
        }

        return $creds_obj
    }
    catch {
        throw [System.UnauthorizedAccessException]::new("Unable to retrieve Unity credentials from [$creds_file_name], which should be located *next to the project root folder*, at [$creds_file_path]",$_.Exception)
    }

    # I'm so tired...but I just can't bring myself to delete this...because it lines up so nicely and the colors are so nice...
    # $cred_fallbacks = [ordered]@{
    #     package = Get-UnityCredentialsFromFile (Join-Path (Find-UnityPackageRoot) $creds_file_name)
    #     project = Get-UnityCredentialsFromFile (Join-Path (Find-UnityProjectRoot) $creds_file_name)
    #     sibling = Get-UnityCredentialsFromFile (Join-Path (Find-UnityProjectRoot) ".." $creds_file_name)
    # }
}

function Build-Unity() {
    # $unity_executable_path = "C:\Program Files\Unity\Editor\Unity.exe"
    # "C:\Program Files\Unity\Editor\Unity.exe" -batchmode -username name@example.edu.uk -password XXXXXXXXXXXXX -serial E3-XXXX-XXXX-XXXX-XXXX-XXXX –quit
    $creds_obj = Get-UnityCredentials

    $unity_command = "'$unity_executable_path' -batchmode -quit -nographics -executeMethod '$unity_method' -username '$($creds_obj.username)' -password '$($creds_obj.password)'"
    &$unity_command
    # "C:\Program Files\Unity\Editor\Unity.exe" -batchmode -quit -nographics -batchmode -executeMethod BuildAutomator.WebGL
}

function Deploy-Unity() {

}