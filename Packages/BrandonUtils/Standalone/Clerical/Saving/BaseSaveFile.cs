﻿using System;
using System.IO;

using BrandonUtils.Standalone.Strings;

using JetBrains.Annotations;

using Newtonsoft.Json;

namespace BrandonUtils.Standalone.Clerical.Saving {
    public abstract class BaseSaveFile<TData> : ISaveFile<TData> where TData : ISaveData {
        public          FileSystemInfo FileSystemInfo => File;
        public          FileInfo       File           { get; }
        public abstract string         Nickname       { get; }
        public          ISaveFolder    SaveFolder     { get; }
        public          TData          Data           { get; internal set; }
        public abstract DateTime       TimeStamp      { get; }

        protected BaseSaveFile(
            [NotNull]   ISaveFolder folder,
            [NotNull]   FileInfo    file,
            [CanBeNull] TData       data
        ) {
            folder.MustBeParentOf(file);
            File       = file   ?? throw new ArgumentNullException(nameof(file));
            SaveFolder = folder ?? throw new ArgumentNullException(nameof(folder));
            Data       = data;
        }

        protected BaseSaveFile(
            [NotNull]   ISaveFolder folder,
            [NotNull]   string      relativePathToFile,
            [CanBeNull] TData       data
        )
            : this(
                folder ?? throw new ArgumentNullException(nameof(folder)),
                new FileInfo(folder.Directory.GetChildPath(relativePathToFile.MustNotBeBlank())),
                data
            ) { }

        public ISaveFile<TData> Save(DuplicateFileResolution duplicateFileResolution, JsonSerializerSettings jsonSettings = default) {
            File.Serialize(Data, duplicateFileResolution);
            return this;
        }

        public ISaveFile<TData> Save(SaveManagerSettings saveSettings = default) {
            saveSettings ??= new SaveManagerSettings();
            Save(saveSettings.DuplicateFileResolution, saveSettings.JsonSerializerSettings);
            return this;
        }

        public ISaveFile<TData> Load(SaveManagerSettings saveSettings = default) {
            saveSettings ??= new SaveManagerSettings();
            Data         =   File.Deserialize<TData>(saveSettings.JsonSerializerSettings);
            return this;
        }

        public override string ToString() {
            return $"📄 {File.ToUri()}";
        }
    }
}