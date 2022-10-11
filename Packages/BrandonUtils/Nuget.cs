using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

#nullable enable

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

    public string[] Frameworks => EnumerateAssemblies().Select(it => it.Framework).ToArray();

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

    private string _GetPackageName()
    {
        var uniqueName = EnumerateAssemblies().Select(it => it.Name).Distinct().Single();
        return Path.GetFileNameWithoutExtension(uniqueName);
    }

    private string _GetPackageVersion()
    {
        if (NupkgFile.Name.StartsWith(Name) == false)
        {
            throw new InvalidOperationException($"The nupkg {NupkgFile.Name} doesn't start with {Name}!");
        }

        var nupkgBaseName = Path.GetFileNameWithoutExtension(NupkgFile.Name);

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

    public void RemoveExtraFrameworks(string targetFramework){
        if(Frameworks.Contains(targetFramework) == false){
            throw new InvalidOperationException($"The Nuget package {Name} wasn't installed with the framework {targetFramework}!");
        }

        var toRemove = Assemblies.Where(it => it.Framework != targetFramework);

        foreach(var f in toRemove){
            System.Console.WriteLine($"Removing: {f}");
            f.Delete();
        }
    }

    public string Colorize(string typeStyle, string versionStyle, string resetStyle = "\u001b[m"){
        return $"{resetStyle}[{typeStyle}{Name}{resetStyle}, {versionStyle}{Version}{resetStyle}]";
    }

    public override string ToString()
    {
        return $"{Name}, {Version}";
    }
}

/// <summary>
/// Describes a specific <see cref="DllFile"/> for a <see cref="NugetPackage"/>, which corresponds to a particular <see cref="Framework"/> <i>(<c>netstandard2.0, net47, etc.</c>)</i>
/// </summary>
public class NugetAssembly
{
    public FileInfo DllFile { get; }
    public string Name => DllFile.Name;
    public DirectoryInfo Folder => DllFile.Directory ?? throw new ArgumentNullException(nameof(Folder));
    public string Framework => Folder.Name;

    public NugetAssembly(FileInfo dllFile)
    {
        DllFile = Nuget.ValidateFile(dllFile, ".dll");
    }

    /// <summary>
    /// <see cref="FileSystemInfo.Delete"/>s everything in the <see cref="Folder"/> AND the <see cref="Folder"/>'s <see cref="Nuget.GetMetaFile(FileSystemInfo)"/>.
    /// </summary>
    public void Delete() => Nuget.DeleteWithMetas(Folder);

    public override string ToString() => $"{Framework}/{DllFile.Name}";
}