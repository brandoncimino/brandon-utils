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
    }
}