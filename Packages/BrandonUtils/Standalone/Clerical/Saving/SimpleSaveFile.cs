using System;
using System.IO;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Clerical.Saving {
    public class SimpleSaveFile<TData> : BaseSaveFile<TData> where TData : ISaveData {
        public override string   Nickname  { get; }
        public override DateTime TimeStamp => File.LastAccessTime;

        public SimpleSaveFile(
            [NotNull]   ISaveFolder saveFolder,
            [NotNull]   FileInfo    fileInfo,
            [CanBeNull] TData       data = default
        ) : base(
            saveFolder,
            fileInfo,
            data
        ) { }

        public SimpleSaveFile(
            [NotNull]   ISaveFolder saveFolder,
            [NotNull]   string      nickname,
            [NotNull]   string      extension = SaveFileName.DefaultExtension,
            [CanBeNull] TData       data      = default
        ) : this(
            saveFolder,
            new FileInfo($"{nickname}{extension}"),
            data
        ) {
            Nickname = nickname ?? throw new ArgumentNullException(nameof(nickname));
        }
    }
}