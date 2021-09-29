using System;
using System.Collections.Generic;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Clerical.Saving {
    public interface ISaveSlot<TData> where TData : ISaveData {
        [NotNull] public string     Nickname   { get; }
        [NotNull] public SaveFolder SaveFolder { get; }

        [NotNull]
        [ItemNotNull]
        public IEnumerable<SaveFile<TData>> EnumerateSaveFiles();

        [CanBeNull]
        public ISaveFile<TData> LatestFile();

        public int SaveFileCount { get; }

        [NotNull]
        public ISaveFile<TData> Save(TData saveData, DateTime now);
    }
}