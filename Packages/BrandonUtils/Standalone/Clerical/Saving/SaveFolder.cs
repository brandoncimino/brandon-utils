using System.Collections.Generic;
using System.IO;
using System.Linq;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Clerical.Saving {
    [PublicAPI]
    public class SaveFolder : CustomDirectoryInfo {
        private const              string SaveFolderName = "SaveData";
        [NotNull] private readonly string PersistentDataPath;
        [NotNull] private readonly string GameName;

        public SaveFolder([NotNull] string persistentDataPath, [NotNull] string gameName) {
            PersistentDataPath = persistentDataPath;
            GameName           = gameName;
        }

        private string FolderPath => Path.Combine(
            PersistentDataPath,
            SaveFolderName,
            GameName
        );

        public override DirectoryInfo Directory => new DirectoryInfo(FolderPath);

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