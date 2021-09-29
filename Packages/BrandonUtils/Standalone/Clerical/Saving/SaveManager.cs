﻿using System;
using System.IO;
using System.Linq;

using BrandonUtils.Standalone.Strings;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Clerical.Saving {
    public class SaveManager<TData> where TData : ISaveData {
        private readonly SaveFolder          SaveFolder;
        public           SaveManagerSettings Settings;

        public SaveManager(SaveFolder saveFolder) {
            SaveFolder = saveFolder;
        }

        public SaveFile<TData> GetSaveFile([NotNull] string nickname, DateTime timeStamp) {
            if (nickname == null) {
                throw new ArgumentNullException(nameof(nickname));
            }

            return new SaveFile<TData>(SaveFolder, nickname, timeStamp, Settings.SaveFileExtension);
        }

        public IOrderedEnumerable<SaveFile<TData>> EnumerateSaveFiles([CanBeNull] string nickname = "*") {
            nickname ??= "*";
            var searchPattern = new SaveFileName() {
                Nickname      = nickname,
                FullExtension = Settings.SaveFileExtension
            }.GetFileSearchPattern();

            return SaveFolder.EnumerateFiles(searchPattern, SearchOption.TopDirectoryOnly)
                             .Where(it => it.BaseName().Matches(SaveFileName.BaseFileNamePattern))
                             .Select(it => new SaveFile<TData>(SaveFolder, it))
                             .OrderByDescending(it => it.TimeStamp);
        }

        internal SaveFileName GetSaveFileName(string nickname, DateTime timeStamp) {
            return new SaveFileName() {
                Nickname      = nickname,
                TimeStamp     = timeStamp,
                FullExtension = Settings.SaveFileExtension
            };
        }
    }
}