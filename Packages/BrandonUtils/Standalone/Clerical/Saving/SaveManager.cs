using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using BrandonUtils.Standalone.Strings;

namespace BrandonUtils.Standalone.Clerical.Saving {
    public class SaveManager<TData> where TData : ISaveData {
        private readonly SaveFolder SaveFolder;
        public           string     AutoSaveName      { get; set; } = "AutoSave";
        public           string     SaveFileExtension { get; set; } = SaveFileName.DefaultExtension;
        public           int        BackupSaveSlots   { get; set; } = 10;

        public SaveManager(SaveFolder saveFolder) {
            SaveFolder = saveFolder;
        }

        public SaveFile<TData> GetSaveFile(string nickname, DateTime timeStamp) {
            return new SaveFile<TData>(SaveFolder, nickname, timeStamp, SaveFileExtension);
        }

        public IEnumerable<SaveFile<TData>> GetAllSaveFiles(string nicknamePattern = "*") {
            return SaveFolder.EnumerateFiles($"{nicknamePattern}_*{SaveFileExtension}", SearchOption.TopDirectoryOnly)
                             .Where(it => it.BaseName().Matches(SaveFileName.BaseFileNamePattern))
                             .Select(it => new SaveFile<TData>(it));
        }

        internal SaveFileName GetSaveFileName(string nickname, DateTime timeStamp) {
            return new SaveFileName() {
                Nickname      = nickname,
                TimeStamp     = timeStamp,
                FullExtension = SaveFileExtension
            };
        }
    }
}