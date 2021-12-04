using System.Collections.Generic;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Clerical.Saving {
    public interface ISaveFolder : IHasDirectoryInfo {
        [NotNull] public string PersistentDataPath             { get; }
        [NotNull] public string RelativePathFromPersistentData { get; }

        [NotNull, ItemNotNull]
        public IEnumerable<ISaveFile<TData>> EnumerateSaveFiles<TData>(string searchPattern) where TData : ISaveData;
    }
}