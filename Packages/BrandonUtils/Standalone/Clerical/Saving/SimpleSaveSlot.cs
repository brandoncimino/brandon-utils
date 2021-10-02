using System;
using System.Collections.Generic;
using System.Linq;

using BrandonUtils.Standalone.Collections;
using BrandonUtils.Standalone.Strings;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Clerical.Saving {
    /// <summary>
    /// The most basic implementation of <see cref="ISaveSlot{TData}"/>:
    /// <ul>
    /// <li>Serializes a <b>new</b> <see cref="ISaveFile{TData}"/> whenever <see cref="Save"/> is called.</li>
    /// <li>Throws an error if the <see cref="ISaveFile{TData}"/> already exists.</li>
    /// </ul>
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    [PublicAPI]
    public class SimpleSaveSlot<TData> : ISaveSlot<TData> where TData : ISaveData {
        public string              Nickname   { get; }
        public SaveManagerSettings Settings   { get; }
        public SaveFolder          SaveFolder { get; }

        [NotNull, ItemNotNull]
        private readonly string[] _relativePath;
        public string[] RelativePath => _relativePath.Copy();

        [NotNull] private readonly SaveFileName FileNameTemplate;
        private                    string       FileSearchPattern => FileNameTemplate.GetFileSearchPattern();

        public SimpleSaveSlot(
            [NotNull]   SaveFolder          saveFolder,
            [NotNull]   string              nickname,
            [CanBeNull] SaveManagerSettings settings = default
        ) {
            if (string.IsNullOrWhiteSpace(nickname)) {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(nickname));
            }

            settings      ??= new SaveManagerSettings();
            SaveFolder    =   saveFolder ?? throw new ArgumentNullException(nameof(saveFolder));
            Nickname      =   nickname;
            Settings      =   settings;
            _relativePath =   new[] { nickname };

            FileNameTemplate = new SaveFileName {
                Nickname      = nickname,
                FullExtension = settings.SaveFileExtension,
            };
        }

        public IEnumerable<SaveFile<TData>> EnumerateSaveFiles() {
            return SaveFolder.EnumerateSaveFiles<TData>(FileSearchPattern);
        }

        public ISaveFile<TData> LatestFile() {
            return EnumerateSaveFiles().First();
        }

        public int SaveFileCount => EnumerateSaveFiles().Count();

        public ISaveFile<TData> Save(TData saveData, DateTime now) {
            var saveFile = new SaveFile<TData>(SaveFolder, Nickname, now, Settings.SaveFileExtension, saveData);
            saveFile.Save(Settings);
            return saveFile;
        }

        public override string ToString() {
            return $"📥 {GetType().Prettify()}: \"{Nickname}\"";
        }
    }
}