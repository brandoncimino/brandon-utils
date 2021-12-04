using System.IO;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Clerical {
    public interface IHasFileSystemInfo {
        [NotNull] public FileSystemInfo FileSystemInfo { get; }
    }
}