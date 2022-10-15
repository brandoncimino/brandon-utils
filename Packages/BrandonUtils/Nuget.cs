using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Runtime.Versioning;

#nullable enable

public enum DotNetFlavor
{
    Standard,
    Core,
    Other
}

/// <summary>
/// General utilities for interacting with files and stuff
/// </summary>
public static class Clerk {
    private static DirectoryInfo ToDir(this string s) => new DirectoryInfo(s);

    private static DirectoryInfo? GetFolder(this FileSystemInfo fileOrDirectory) => fileOrDirectory switch{
        FileInfo f => f.Directory,
        DirectoryInfo d => d,
        _ => Path.GetDirectoryName(fileOrDirectory.FullName)?.ToDir()
    };


    public static DirectoryInfo? FindUpward(FileSystemInfo child, Func<DirectoryInfo, bool> predicate, int depth = 10){
        if(depth < 0){
            return null;
        }

        var folder = child.GetFolder();
        if(folder == null){
            return null;
        }
        if(predicate(folder)){
            return folder;
        }
        else{
            return FindUpward(child, predicate, depth-1);
        }
    }
}

/// <summary>
/// Contains static utility methods to work with <see cref="NugetPackage"/>s and <see cref="NugetAssembly"/>-s.
/// </summary>
public static class Nuget
{
    private static Lazy<NugetPackage[]> _packages = LazyPackages();

    /// <summary>
    /// A <see cref="Lazy{T}"/> reference to the <see cref="NugetPackage"/>s in the <see cref="Directory.GetCurrentDirectory()"/>.
    /// </summary>
    /// <remarks>
    /// An update can be forced using <see cref="Refresh"/>.
    /// </remarks>
    public static NugetPackage[] Packages => _packages.Value;

    /// <summary>
    /// Resets the value of <see cref="Packages"/> so that it will be up-to-date.
    /// </summary>
    public static void Refresh()
    {
        _packages = LazyPackages();
    }

    /// <summary>
    /// Locates all of the <c>.nupkg</c> files under the given <see cref="DirectoryInfo"/> as <see cref="NugetPackage"/>s.
    /// </summary>
    /// <param name="folder">The folder that contains the <c>.nupkg</c> files. <i>(📎 Defaults to <see cref="Directory.GetCurrentDirectory"/>)</i></param>
    /// <returns>a lazy enumeration of the located <see cref="NugetPackage"/>s</returns>
    public static IEnumerable<NugetPackage> EnumeratePackages(DirectoryInfo? folder = default)
    {
        folder ??= new DirectoryInfo(Directory.GetCurrentDirectory());
        return folder.EnumerateFiles("*.nupkg", SearchOption.AllDirectories)
            .Where(it => it.FullName.Contains(".idea") == false)
            .Select(it => new NugetPackage(it));
    }

    private static Lazy<NugetPackage[]> LazyPackages() => new Lazy<NugetPackage[]>(() => EnumeratePackages().ToArray());

    /// <summary>
    /// Validates that a <see cref="FileInfo"/> exists and has the correct <see cref="FileSystemInfo.Extension"/>.
    /// </summary>
    /// <param name="file">A <see cref="FileInfo"/> that <i>might</i> be valid</param>
    /// <exception cref="NullReferenceException">If the <paramref name="file"/> is <c>null</c></exception>
    /// <exception cref="FileNotFoundException">If the <paramref name="file"/> doesn't <see cref="FileSystemInfo.Exists"/></exception>
    /// <exception cref="ArgumentException">If the <paramref name="file"/>'s <see cref="FileSystemInfo.Extension"/> isn't <paramref name="requiredExtension"/></exception>
    public static FileInfo ValidateFile(FileInfo? file, string requiredExtension)
    {
        if (file == null)
        {
            throw new ArgumentNullException(nameof(file));
        }

        file.Refresh();
        if (file.Exists == false)
        {
            throw new FileNotFoundException($"The file {file} doesn't exist!");
        }

        if (file.Extension != requiredExtension)
        {
            throw new ArgumentException($"The file {file.Name} doesn't have the `{requiredExtension}` extension!");
        }

        return file;
    }

    /// <param name="fileSystemInfo">a file or directory</param>
    /// <returns>the corresponding Unity <c>.meta</c> file, if one exists; otherwise, <c>null</c></returns>
    public static FileInfo? GetMetaFile(FileSystemInfo fileSystemInfo)
    {
        var metaFile = new FileInfo($"{fileSystemInfo.FullName}.meta");
        return metaFile.Exists ? metaFile : null;
    }

    /// <summary>
    /// Deletes both a <see cref="FileSystemInfo"/> and its corresponding <see cref="GetMetaFile(FileSystemInfo)"/>, if one exists
    /// </summary>
    /// <param name="fileSystemInfo">a file or folder</param>
    public static void DeleteWithMetas(FileSystemInfo fileSystemInfo)
    {
        GetMetaFile(fileSystemInfo)?.Delete();

        if (fileSystemInfo is DirectoryInfo dir)
        {
            dir.Delete(true);
        }
        else
        {
            fileSystemInfo.Delete();
        }
    }

    #region Parsing framework versions

    private static readonly Regex StandardIdentifierPattern = new Regex(@"^\.?net\s*standard$", RegexOptions.IgnoreCase);
    private static readonly Regex CoreIdentifierPattern = new Regex(@"\.?net$", RegexOptions.IgnoreCase);

#nullable enable
    public static DotNetFlavor ParseDotNetFlavor(string? flavor)
    {
        if (string.IsNullOrWhiteSpace(flavor))
        {
            return DotNetFlavor.Other;
        }
        else if (StandardIdentifierPattern.IsMatch(flavor))
        {
            return DotNetFlavor.Standard;
        }
        else if (CoreIdentifierPattern.IsMatch(flavor))
        {
            return DotNetFlavor.Core;
        }
        else
        {
            return DotNetFlavor.Other;
        }
    }

    public static FrameworkName ParseFramework(string framework)
    {
        var match = Regex.Match(framework, @"(?<identifier>[a-z]+)\s*(?<version>.+)");
        if (match.Success)
        {
            var identifierString = match.Groups["identifier"].Value;
            var versionString = match.Groups["version"].Value;

            var flavor = ParseDotNetFlavor(identifierString);
            var version = ParseVersion(versionString);
            return new FrameworkName(flavor.ToString(), version);
        }

        return new FrameworkName(DotNetFlavor.Other.ToString(), new Version());
    }


    /// <summary>
    /// The <see cref="Version"/> constructors throw errors if you pass in -1, even though that's a completely valid value for <see cref="Version.Build"/>
    /// and <see cref="Version.Revision"/>, making this method infinitely more obnoxious than it should be.
    /// </summary>
    /// <param name="version"></param>
    /// <returns></returns>
    private static Version ParseVersion(string version)
    {
        var splits = Regex.Split(version, @"\D");

        if(int.TryParse(splits.ElementAtOrDefault(0), out var major) == false){
            throw new ArgumentOutOfRangeException(nameof(version), version, $"Input {version} didn't have a major version number!");
        }

        var minor = int.TryParse(splits.ElementAtOrDefault(1), out var min) ? min : 0;

        if(int.TryParse(splits.ElementAtOrDefault(2), out var patch)){
            return new Version(major, minor, patch);
        }
        else{
            return new Version(major, minor);
        }
    }

    #endregion

}

/// <summary>
/// Describes a downloaded Nuget package and its related files.
/// </summary>
public class NugetPackage
{
    /// <summary>
    /// The actual <c>.nupkg</c> file.
    /// </summary>
    public FileInfo NupkgFile { get; }

    /// <summary>
    /// The <see cref="FileSystemInfo.FullName"/> of the <see cref="NupkgFile"/>.
    /// </summary>
    /// <remarks>
    /// This property exists to facilitate <c>ValueFromPipelineFromPropertyName</c> in Powershell.
    /// </remarks>
    public string Path => NupkgFile.FullName;

    private readonly Lazy<string> _name;
    /// <summary>
    /// The name of the package, as it appears in Nuget search results, <c>.dll</c> file names, etc.
    /// </summary>
    /// <remarks>
    /// Note that this is <b>NOT</b> the <see cref="FileSystemInfo.Name"/> of the <see cref="NupkgFile"/> - the <see cref="NupkgFile"/> contains the <see cref="Version"/> as well!
    /// </remarks>
    public string Name => _name.Value;

    private readonly Lazy<string> _version;
    /// <summary>
    /// The version of the package.
    /// </summary>
    /// <remarks>
    /// This is combined with the <see cref="Name"/> to produce the <see cref="NupkgFile"/>'s <see cref="FileSystemInfo.Name"/>.
    /// </remarks>
    public string Version => _version.Value;

    /// <summary>
    /// All of the <see cref="NugetAssembly"/>-s that are currently present for this <see cref="NugetPackage"/>.
    /// </summary>
    public NugetAssembly[] Assemblies => EnumerateAssemblies().ToArray();

    public FrameworkName[] Frameworks => EnumerateAssemblies().Select(it => it.Framework).ToArray();

    /// <summary>
    /// Constructs a <see cref="NugetPackage"/> from an existing <see cref="NupkgFile"/>.
    /// </summary>
    /// <inheritdoc cref=Nuget.ValidateFile(FileInfo?, string)/>
    public NugetPackage(FileInfo nupkgFile)
    {
        NupkgFile = Nuget.ValidateFile(nupkgFile, ".nupkg");
        _name = new Lazy<string>(_GetPackageName);
        _version = new Lazy<string>(_GetPackageVersion);
    }

    /// <summary>
    /// Constructs a <see cref="NugetPackage"/> from a <see cref="DirectoryInfo"/> that <i>contains</i> a <see cref="NupkgFile"/>.
    /// </summary>
    /// <param name="nupkgDirectory">the <see cref="DirectoryInfo"/> that contains the <see cref="NupkgFile"/></param>
    /// <returns>a new <see cref="NugetPackage"/></returns>
    /// <exception cref="ArgumentException">if the <see cref="DirectoryInfo"/> doesn't contain a file with a <c>.nupkg</c> extension</>
    public NugetPackage(DirectoryInfo nupkgDirectory) : this(_FindNupkg(nupkgDirectory)) { }

    private static FileInfo _FindNupkg(DirectoryInfo dir){
        return dir.EnumerateFiles("*.nupkg").FirstOrDefault() ?? throw new ArgumentException($"The directory [{dir.Name}] didn't contain a .nupkg file!");
    }

    private string _GetPackageName()
    {
        var uniqueName = EnumerateAssemblies().Select(it => it.Name).Distinct().SingleOrDefault(default(string));
        if (uniqueName == null)
        {
            throw new InvalidOperationException("Unable to determine the package name because there were no installed assemblies for it!");
        }
        return System.IO.Path.GetFileNameWithoutExtension(uniqueName);
    }

    private string _GetPackageVersion()
    {
        if (NupkgFile.Name.StartsWith(Name) == false)
        {
            throw new InvalidOperationException($"The nupkg {NupkgFile.Name} doesn't start with {Name}!");
        }

        var nupkgBaseName = System.IO.Path.GetFileNameWithoutExtension(NupkgFile.Name);

        var version = nupkgBaseName.Substring(Name.Length).TrimStart('.');
        if (string.IsNullOrWhiteSpace(version))
        {
            throw new InvalidOperationException($"Trimming the nupkg base name [{nupkgBaseName}] by removing [{Name}] resulted in a blank string!");
        }

        return version;
    }

    private IEnumerable<NugetAssembly> EnumerateAssemblies()
    {
        return NupkgFile.Directory?.GetFiles("*.dll", SearchOption.AllDirectories)
        .Select(static it => new NugetAssembly(it))
        ?? Enumerable.Empty<NugetAssembly>();
    }

    public bool HasFramework(string framework) => Frameworks.Contains(Nuget.ParseFramework(framework));

    public string Colorize(string typeStyle, string versionStyle, string resetStyle = "\u001b[m")
    {
        return $"{resetStyle}[{typeStyle}{Name}{resetStyle}, {versionStyle}{Version}{resetStyle}]";
    }

    public override string ToString() => NupkgFile.FullName;

    public static implicit operator NugetPackage(FileInfo fileInfo) => new(fileInfo);
    public static implicit operator NugetPackage(DirectoryInfo directoryInfo) => new(directoryInfo);
}

/// <summary>
/// Describes a specific <see cref="DllFile"/> for a <see cref="NugetPackage"/>, which corresponds to a particular <see cref="Framework"/> <i>(<c>netstandard2.0, net47, etc.</c>)</i>
/// </summary>
public class NugetAssembly
{
    public FileInfo DllFile { get; }
    public string Name => DllFile.Name;
    public DirectoryInfo Folder => DllFile.Directory ?? throw new ArgumentNullException(nameof(Folder));
    public DirectoryInfo LibFolder => DllFile.Directory?.Parent ?? throw new ArgumentNullException(nameof(LibFolder));
    private Lazy<NugetPackage> _package;
    public NugetPackage Package => _package.Value;
    public FrameworkName Framework => Nuget.ParseFramework(Folder.Name);
    public string Path => DllFile.FullName;

    public NugetAssembly(FileInfo dllFile, NugetPackage? package = default)
    {
        DllFile = Nuget.ValidateFile(dllFile, ".dll");
        if(package != null){
            _package = new Lazy<NugetPackage>(package);
        }
        else{
            _package = new Lazy<NugetPackage>(_FindPackage);
        }
    }

    private NugetPackage _FindPackage(){
        return new NugetPackage(Clerk.FindUpward(DllFile, dir => dir.EnumerateFiles("*.nupkg").Any()) ?? throw new InvalidOperationException("Couldn't find a parent directory containing a .nuget file!"));
    }

    public bool HasFramework(string framework)
    {
        return Framework == Nuget.ParseFramework(framework);
    }

    /// <summary>
    /// <see cref="FileSystemInfo.Delete"/>s everything in the <see cref="Folder"/> AND the <see cref="Folder"/>'s <see cref="Nuget.GetMetaFile(FileSystemInfo)"/>.
    /// </summary>
    public void Delete() => Nuget.DeleteWithMetas(Folder);

    public override string ToString() => DllFile.FullName;
}