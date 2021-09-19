using System;
using System.IO;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Clerical.Saving {
    [PublicAPI]
    public class SaveFile<TData> : CustomFileInfo, ISaveFile<TData> where TData : ISaveData {
        [NotNull] private readonly SaveFileName _saveFileName;

        [NotNull] public string Nickname => _saveFileName.Nickname;
        public DateTime TimeStamp {
            get => _saveFileName.TimeStamp;
            set => _saveFileName.TimeStamp = value;
        }

        public SaveFolder SaveFolder { get; }

        public override FileInfo File { get; }
        public          TData    Data { get; internal set; }

        void ISaveFile<TData>.SetDataInternal(TData data) {
            Data = data;
        }

        public SaveFile([NotNull] SaveFolder folder, [NotNull] string nickname, DateTime timeStamp, string extension = SaveFileName.DefaultExtension)
            : this(
                folder,
                new SaveFileName() {
                    Nickname      = nickname,
                    TimeStamp     = timeStamp,
                    FullExtension = extension
                }
            ) { }

        internal SaveFile([NotNull] SaveFolder folder, [NotNull] SaveFileName saveFileName) {
            SaveFolder    = folder;
            _saveFileName = saveFileName;
            File          = new FileInfo(Path.Combine(SaveFolder.FullName, _saveFileName.Rendered));
        }

        public SaveFile([NotNull] FileInfo fileInfo) {
            File          = fileInfo;
            _saveFileName = SaveFileName.Parse(fileInfo);
        }
    }
}