using System;
using System.IO;

using JetBrains.Annotations;

using Newtonsoft.Json;

namespace BrandonUtils.Standalone.Clerical.Saving {
    [PublicAPI]
    public class SaveFile<TData> : CustomFileInfo, ISaveFile<TData> where TData : ISaveData {
        [NotNull] private readonly SaveFileName _saveFileName;

        public string Nickname => _saveFileName.Nickname;

        public DateTime TimeStamp => _saveFileName.TimeStamp;

        public SaveFolder SaveFolder { get; }

        public override FileInfo File { get; }

        public TData Data { get; private set; }

        public SaveFile(
            [NotNull] SaveFolder folder,
            [NotNull] string     nickname,
            DateTime             timeStamp,
            [CanBeNull] string   extension = SaveFileName.DefaultExtension,
            TData                data      = default
        )
            : this(
                folder,
                new SaveFileName {
                    Nickname      = nickname,
                    TimeStamp     = timeStamp,
                    FullExtension = extension ?? SaveFileName.DefaultExtension
                },
                data
            ) { }

        internal SaveFile(
            [NotNull]   SaveFolder   folder,
            [NotNull]   SaveFileName saveFileName,
            [CanBeNull] TData        data
        ) {
            SaveFolder    = folder       ?? throw new ArgumentNullException(nameof(folder));
            _saveFileName = saveFileName ?? throw new ArgumentNullException(nameof(saveFileName));
            File          = new FileInfo(Path.Combine(SaveFolder.FullName, _saveFileName.Rendered));
            Data          = data;
        }

        public SaveFile([NotNull] SaveFolder folder, [NotNull] FileInfo fileInfo) {
            if (folder == null) {
                throw new ArgumentNullException(nameof(folder));
            }

            if (fileInfo == null) {
                throw new ArgumentNullException(nameof(fileInfo));
            }

            folder.Directory.MustBeParentOf(fileInfo);
            SaveFolder    = folder;
            File          = fileInfo;
            _saveFileName = SaveFileName.Parse(fileInfo);
        }

        public void Save(SaveManagerSettings saveSettings = default) {
            saveSettings ??= new SaveManagerSettings();
            Save(saveSettings.DuplicateFileResolution.Value, saveSettings.JsonSerializerSettings.Value);
        }

        public void Save(DuplicateFileResolution duplicateFileResolution, JsonSerializerSettings jsonSettings = default) {
            File.Serialize(Data, duplicateFileResolution, jsonSettings);
        }

        public ISaveFile<TData> Load(SaveManagerSettings saveSettings = default) {
            saveSettings ??= new SaveManagerSettings();
            Data         =   File.Deserialize<TData>(saveSettings.JsonSerializerSettings.Value);
            return this;
        }

        public override string ToString() {
            return $"📄 {File.ToUri()}";
        }
    }
}