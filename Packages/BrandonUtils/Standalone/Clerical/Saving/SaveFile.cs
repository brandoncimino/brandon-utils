using System;
using System.IO;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Clerical.Saving {
    [PublicAPI]
    public class SaveFile : CustomFileInfo {
        private readonly SaveFileName _saveFileName;

        [NotNull] public string   Nickname  => _saveFileName.Nickname;
        public           DateTime TimeStamp => _saveFileName.TimeStamp;

        public SaveFolder SaveFolder { get; }

        public override FileInfo FileInfo { get; }

        public SaveFile([NotNull] SaveFolder folder, [NotNull] string nickname, DateTime timeStamp, string extension = SaveFileName.DefaultExtension)
            : this(
                folder,
                new SaveFileName() {
                    Nickname      = nickname,
                    TimeStamp     = timeStamp,
                    FullExtension = extension
                }
            ) { }

        public SaveFile([NotNull] SaveFolder folder, [NotNull] SaveFileName saveFileName) {
            SaveFolder = folder;
            FileInfo   = new FileInfo(Path.Combine(SaveFolder.FullName, _saveFileName.Rendered));
        }

        public SaveFile([NotNull] FileInfo fileInfo) {
            FileInfo      = fileInfo;
            _saveFileName = SaveFileName.Parse(fileInfo);
        }
    }
}