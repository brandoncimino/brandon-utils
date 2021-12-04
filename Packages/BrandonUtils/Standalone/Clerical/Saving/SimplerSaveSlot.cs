using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Clerical.Saving {
    public class SimplerSaveSlot<TData> : ISaveSlot<TData> where TData : ISaveData {
        public string              Nickname              { get; }
        public ISaveFolder         SaveFolder            { get; }
        public string[]            RelativePath          => Array.Empty<string>();
        public int                 SaveFileCount         => EnumerateSaveFiles().Count();
        public SaveManagerSettings Settings              { get; }
        public string              SaveFileSearchPattern => MyFileName;

        [NotNull] private string   MyFileName => $"{Nickname}{Settings.SaveFileExtension}";
        [NotNull] private string   MyFilePath => SaveFolder.Directory.GetChildPath(MyFileName);
        [NotNull] private FileInfo MyFile     => new FileInfo(MyFilePath);

        public SimplerSaveSlot(
            SaveFolder                     saveFolder,
            [NotNull] string               nickname,
            SaveManagerSettings? settings = default
        ) {
            if (string.IsNullOrWhiteSpace(nickname)) {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(nickname));
            }

            SaveFolder = saveFolder;
            Nickname   = nickname;
            Settings   = settings ?? new SaveManagerSettings();
        }

        public IEnumerable<ISaveFile<TData>> EnumerateSaveFiles() {
            return SaveFolder.EnumerateSaveFiles<TData>(SaveFileSearchPattern);
        }

        [NotNull]
        public ISaveFile<TData> LatestFile() {
            return new SimpleSaveFile<TData>(SaveFolder, MyFile);
        }

        public ISaveFile<TData> Save(TData saveData, DateTime now) {
            var newFile = new SimpleSaveFile<TData>(SaveFolder, Nickname, Settings.SaveFileExtension) {
                Data = saveData,
            };

            return newFile.Save(Settings);
        }
    }
}