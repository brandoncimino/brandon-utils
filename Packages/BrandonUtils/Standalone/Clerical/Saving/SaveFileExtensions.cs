﻿using System;
using System.IO;

using BrandonUtils.Standalone.Strings;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Clerical.Saving {
    public static class SaveFileExtensions {
        internal static void MustExist(this ISaveFile<ISaveData>? saveFile) {
            if (saveFile == null) {
                throw new ArgumentNullException(nameof(saveFile));
            }

            saveFile.FileSystemInfo.Refresh();
            if (saveFile.FileSystemInfo.Exists == false) {
                throw new FileNotFoundException($"The file referenced by the {saveFile.GetType().Prettify()} {saveFile.Nickname} does not exist!");
            }
        }
    }
}