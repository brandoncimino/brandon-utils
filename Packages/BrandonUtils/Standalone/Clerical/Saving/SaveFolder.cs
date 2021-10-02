using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Clerical.Saving {
    [PublicAPI]
    public class SaveFolder : CustomDirectoryInfo {
        public const string SaveFolderName = "SaveData";

        [NotNull] public string PersistentDataPath { get; set; }
        [NotNull] public string GameName           { get; set; }

        [NotNull] public override DirectoryInfo Directory => new DirectoryInfo(FolderPath);


        public SaveFolder(
            [NotNull] string persistentDataPath,
            [NotNull] string gameName
        ) {
            if (string.IsNullOrWhiteSpace(persistentDataPath)) {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(persistentDataPath));
            }

            if (string.IsNullOrWhiteSpace(gameName)) {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(gameName));
            }

            PersistentDataPath = persistentDataPath;
            GameName           = gameName;
        }

        [NotNull]
        private string FolderPath => BPath.JoinPath(
            PersistentDataPath,
            SaveFolderName,
            GameName
        );


        [NotNull]
        public IEnumerable<SaveFile<T>> EnumerateSaveFiles<T>(string searchPattern) where T : ISaveData {
            if (Exists) {
                return EnumerateFiles(searchPattern, SearchOption.TopDirectoryOnly)
                       .Select(it => new SaveFile<T>(this, it))
                       .OrderByDescending(it => it.TimeStamp);
            }
            else {
                return Enumerable.Empty<SaveFile<T>>();
            }
        }
    }
}