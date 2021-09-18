using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using BrandonUtils.Standalone.Strings;

namespace BrandonUtils.Standalone.Clerical.Saving {
    public class SaveManager {
        private readonly SaveFolder SaveFolder;
        public           string     AutoSaveName      { get; set; } = "AutoSave";
        public           string     SaveFileExtension { get; set; } = SaveFileName.DefaultExtension;
        public           int        BackupSaveSlots   { get; set; } = 10;

        public SaveManager(SaveFolder saveFolder) {
            SaveFolder = saveFolder;
        }

        public SaveFile GetSaveFile(string nickname, DateTime timeStamp) {
            return new SaveFile(SaveFolder, nickname, timeStamp, SaveFileExtension);
        }

        public IEnumerable<SaveFile> GetAllSaveFiles(string nicknamePattern = "*") {
            return SaveFolder.EnumerateFiles($"{nicknamePattern}_*{SaveFileExtension}", SearchOption.TopDirectoryOnly)
                             .Where(it => it.BaseName().Matches(SaveFileName.BaseFileNamePattern))
                             .Select(it => new SaveFile(it));
        }

        public SaveFileName GetSaveFileName(string nickname, DateTime timeStamp) {
            return new SaveFileName() {
                Nickname      = nickname,
                TimeStamp     = timeStamp,
                FullExtension = SaveFileExtension
            };
        }
    }
}