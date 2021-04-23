<# 
    https://docs.unity3d.com/Manual/CommandLineArguments.html

    .SYNOPSIS
    Butler is downloaded to & expanded in $butler_dir
 #>
function Find-ParentFolder($Path, $ParentName){
    if(Split-Path $Path -Leaf -eq $ParentName){
        return $Path
    }

    return Find-ParentFolder ($Path | Split-Path -Parent) $ParentName
}

function Find-UnityPackageRoot($Path = $PWD){
    try {
        return Join-Path $Path ".." "package.json" -Resolve | Split-Path -Parent
    }
    catch {
        return Find-UnityPackageRoot ($Path | Split-Path)
    }
}

function Find-UnityProjectRoot($Path = $PWD){
    $has_assets_folder = Test-Path (Join-Path $Path "Assets")
    $has_library_folder = Test-Path(Join-Path $Path "Library")

    if($has_assets_folder -and $has_library_folder){
        return $Path
    }

    return Find-UnityProjectRoot ($Path | Split-Path -Parent)
}

# Exit if there is any error in the build script
$ErrorActionPreference = "Stop"
$unity_project_root = Find-UnityProjectRoot

#region Enums
enum OS {
    Windows
    Mac
    Linux
}

enum UnityBuildTarget {

}
#endregion

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

#tag::install-butler[]
function Install-Butler() {
    #region butler constants
    $butler_name = "butler"
    $butler_dir = Join-Path $unity_project_root $butler_name
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


function Build-Unity() {
    Unity -quit -batchmode -executeMethod BuildAutomator.WebGL
}

function Deploy-Unity() {

}