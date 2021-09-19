﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using BrandonUtils.Standalone.Strings;

using JetBrains.Annotations;

using Newtonsoft.Json;

namespace BrandonUtils.Standalone.Clerical {
    [PublicAPI]
    public static class FileInfoExtensions {
        /// <summary>
        /// An extension method made to mimic Powershell's <see cref="FileInfo"/>.<c>BaseName</c> <a href="https://docs.microsoft.com/en-us/powershell/scripting/developer/cmdlet/extending-properties-for-objects?view=powershell-7.1#script-properties">Script Property</a>.
        /// </summary>
        /// <param name="fileSystemInfo"></param>
        /// <returns></returns>
        public static string BaseName(this FileSystemInfo fileSystemInfo) {
            return Path.GetFileNameWithoutExtension(fileSystemInfo.Name);
        }

        #region BPath Extensions

        public static string[] Extensions(this FileSystemInfo fileSystemInfo) {
            return BPath.GetExtensions(fileSystemInfo.Name);
        }

        public static string FullExtension(this FileSystemInfo fileSystemInfo) {
            return BPath.GetFullExtension(fileSystemInfo.Name);
        }

        public static string FileNameWithoutExtensions(this FileSystemInfo fileSystemInfo) {
            return BPath.GetFileNameWithoutExtensions(fileSystemInfo.Name);
        }

        #endregion

        #region System.IO.File Extensions

        #region CreateDirectory

        /// <summary>
        /// Creates <paramref name="fileInfo"/>'s <see cref="FileInfo.Directory"/>.
        /// </summary>
        /// <remarks>
        /// The only difference between <see cref="CreateDirectory"/> and <see cref="EnsureDirectory"/> is what they return:
        /// <ul>
        /// <li><see cref="CreateDirectory"/> → <paramref name="fileInfo"/>.<see cref="FileInfo.Directory"/></li>
        /// <li><see cref="EnsureDirectory"/> → <paramref name="fileInfo"/></li>
        /// </ul>
        /// </remarks>
        /// <param name="fileInfo">a <see cref="FileInfo"/></param>
        /// <returns><paramref name="fileInfo"/>'s <see cref="FileInfo.Directory"/>, whether or not it already existed.</returns>
        /// <exception cref="ArgumentNullException">if <paramref name="fileInfo"/> is null</exception>
        /// <exception cref="ArgumentNullException">if <paramref name="fileInfo"/>.<see cref="Directory"/> is null</exception>
        /// <exception cref="IOException">if the <see cref="DirectoryInfo"/> couldn't be created</exception>
        [NotNull]
        [ContractAnnotation("null => stop")]
        public static DirectoryInfo CreateDirectory([NotNull] this FileInfo fileInfo) {
            if (fileInfo == null) {
                throw new ArgumentNullException(nameof(fileInfo));
            }

            var dir = fileInfo.Directory ?? throw new ArgumentNullException(nameof(fileInfo.Directory), $"The {fileInfo.GetType().Prettify()}.{nameof(fileInfo.Directory)} was null, so we couldn't create it! ({nameof(fileInfo.FullName)}: \"{fileInfo.FullName}\")");
            return Directory.CreateDirectory(dir.FullName);
        }

        /// <inheritdoc cref="CreateDirectory"/>
        /// <returns>this <paramref name="fileInfo"/>, for method chaining</returns>
        [NotNull]
        [ContractAnnotation("null => stop")]
        private static FileInfo EnsureDirectory([NotNull] this FileInfo fileInfo) {
            fileInfo.CreateDirectory();
            return fileInfo;
        }

        #endregion

        #region Read

        /// <seealso cref="File.ReadAllLines(string)"/>
        public static string[] ReadLines(this FileInfo fileInfo) => File.ReadAllLines(fileInfo.FullName);

        /// <seealso cref="File.ReadAllLines(string,Encoding)"/>
        public static string[] ReadLines(this FileInfo fileInfo, Encoding encoding) => File.ReadAllLines(fileInfo.FullName, encoding);

        /// <seealso cref="File.ReadAllText(string)"/>
        public static string Read(this FileInfo fileInfo) => File.ReadAllText(fileInfo.FullName);

        /// <seealso cref="File.ReadAllText(string, Encoding)"/>
        public static string Read(this FileInfo fileInfo, Encoding encoding) => File.ReadAllText(fileInfo.FullName, encoding);

        #endregion

        #region Write

        /// <summary>
        /// A nice wrapper for <see cref="File.WriteAllText(string,string)"/>.
        /// </summary>
        /// <remarks>
        /// This is a wrapper around a static <see cref="File"/> method that:
        /// <ul>
        /// <li>Creates <paramref name="fileInfo"/>'s <see cref="FileInfo.Directory"/> if necessary</li>
        /// <li>Uses this <see cref="FileInfo"/>'s <see cref="FileSystemInfo.FullName"/> as the path</li>
        /// <li>Returns this <see cref="FileInfo"/>, for method chaining</li>
        /// </ul>
        /// </remarks>
        /// <param name="fileInfo">the <see cref="FileInfo"/> being written to</param>
        /// <param name="contents">the text being written to <paramref name="fileInfo"/></param>
        /// <returns>this <see cref="FileInfo"/>, for method chaining</returns>
        public static FileInfo Write(this FileInfo fileInfo, string contents) {
            fileInfo.EnsureDirectory();
            File.WriteAllText(fileInfo.FullName, contents);
            fileInfo.Refresh();
            return fileInfo;
        }

        /// <summary>
        /// A nice wrapper for <see cref="File.WriteAllText(string,string,Encoding)"/>.
        /// </summary>
        /// <remarks>
        /// <inheritdoc cref="Write(System.IO.FileInfo,string)"/>
        /// </remarks>
        /// <param name="fileInfo"><inheritdoc cref="Write(System.IO.FileInfo,string)"/></param>
        /// <param name="contents"><inheritdoc cref="Write(System.IO.FileInfo,string)"/></param>
        /// <param name="encoding">an optional text <see cref="Encoding"/></param>
        /// <returns><inheritdoc cref="Write(System.IO.FileInfo,string)"/></returns>
        public static FileInfo Write(this FileInfo fileInfo, string contents, Encoding encoding) {
            fileInfo.CreateDirectory();
            File.WriteAllText(fileInfo.FullName, contents, encoding);
            fileInfo.Refresh();
            return fileInfo;
        }

        /// <summary>
        /// A nice wrapper for <see cref="File.WriteAllLines(string,System.Collections.Generic.IEnumerable{string})"/>
        /// </summary>
        /// <param name="fileInfo"><inheritdoc cref="Write(System.IO.FileInfo,string)"/></param>
        /// <param name="lines">the <see cref="string"/>s that will be written to <paramref name="fileInfo"/> on <b>separate lines</b></param>
        /// <returns><inheritdoc cref="Write(System.IO.FileInfo,string)"/></returns>
        public static FileInfo WriteLines(this FileInfo fileInfo, IEnumerable<string> lines) {
            fileInfo.CreateDirectory();
            File.WriteAllLines(fileInfo.FullName, lines);
            fileInfo.Refresh();
            return fileInfo;
        }

        /**
         * <inheritdoc cref="WriteLines(System.IO.FileInfo,System.Collections.Generic.IEnumerable{string})"/>
         */
        public static FileInfo WriteLines(this FileInfo fileInfo, IEnumerable<string> lines, Encoding encoding) {
            fileInfo.CreateDirectory();
            File.WriteAllLines(fileInfo.FullName, lines);
            fileInfo.Refresh();
            return fileInfo;
        }

        #region Append

        /// <seealso cref="File.AppendAllText(string,string)"/>
        public static void Append(this FileInfo fileInfo, string contents) => File.AppendAllText(fileInfo.FullName, contents);

        /// <seealso cref="File.AppendAllText(string,string, Encoding)"/>
        public static void Append(this FileInfo fileInfo, string contents, Encoding encoding) => File.AppendAllText(fileInfo.FullName, contents, encoding);

        /// <seealso cref="File.AppendAllLines(string,System.Collections.Generic.IEnumerable{string})"/>
        public static void AppendLines(this FileInfo fileInfo, IEnumerable<string> lines) => File.AppendAllLines(fileInfo.FullName, lines);

        /// <seealso cref="File.AppendAllLines(string,System.Collections.Generic.IEnumerable{string}, Encoding)"/>
        public static void AppendLines(this FileInfo fileInfo, IEnumerable<string> lines, Encoding encoding) => File.AppendAllLines(fileInfo.FullName, lines, encoding);

        #endregion

        #endregion

        #endregion

        #region NewtonsoftJson Extensions

        public static T    Deserialize<T>(this FileInfo fileInfo, [CanBeNull] JsonSerializerSettings settings                                         = default) => JsonConvert.DeserializeObject<T>(fileInfo.Read(), settings);
        public static void Serialize<T>(this   FileInfo fileInfo, [CanBeNull] T                      obj, [CanBeNull] JsonSerializerSettings settings = default) => fileInfo.Write(JsonConvert.SerializeObject(obj, settings!));

        #endregion
    }
}