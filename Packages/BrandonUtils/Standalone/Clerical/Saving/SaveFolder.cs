using System.IO;

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

        public override DirectoryInfo DirectoryInfo => new DirectoryInfo(FolderPath);
    }
}