using System.IO;

using JetBrains.Annotations;

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
    }
}