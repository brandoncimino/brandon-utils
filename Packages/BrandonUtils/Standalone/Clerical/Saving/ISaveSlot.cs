using System;
using System.Collections.Generic;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Clerical.Saving {
    public interface ISaveSlot<TData> where TData : ISaveData {
        [NotNull] public string      Nickname   { get; }
        [NotNull] public ISaveFolder SaveFolder { get; }

        [NotNull, ItemNotNull]
        public string[] RelativePath { get; }

        [NonNegativeValue]
        public int SaveFileCount { get; }

        [NotNull] public string SaveFileSearchPattern { get; }

        [NotNull] public SaveManagerSettings Settings { get; }

        [NotNull]
        [ItemNotNull]
        public IEnumerable<ISaveFile<TData>> EnumerateSaveFiles();

        [CanBeNull]
        public ISaveFile<TData> LatestFile();

        [NotNull]
        public ISaveFile<TData> Save(TData saveData, DateTime now);
    }
}