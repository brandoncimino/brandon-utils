using System;

using BrandonUtils.Standalone.Clerical.Saving;
using BrandonUtils.Testing;

using NUnit.Framework;

using UnityEditor;

using UnityEngine;

namespace BrandonUtils.Tests.EditMode.Clerical {
    public class SaveFileTests {
        private static readonly SaveFolder SaveFolder = new SaveFolder(Application.persistentDataPath, nameof(SaveFileTests));

        private static class Validate {
            public static void SaveFile_Basic<T>(SaveFile<T> saveFile, string expectedName, DateTime expectedTimeStamp) where T : ISaveData {
                AssertAll.Of(
                    saveFile,
                    Has.Property(nameof(saveFile.Nickname)).EqualTo(expectedName),
                    Has.Property(nameof(saveFile.TimeStamp)).EqualTo(expectedTimeStamp),
                    Has.Property(nameof(saveFile.Name)).EqualTo($"{expectedName}_{expectedTimeStamp.Ticks}{SaveFileName.DefaultExtension}")
                );
            }
        }

        [Test]
        public void Save_NullData() {
            var bDay     = new DateTime(1993, 7, 1);
            var today    = DateTime.Today;
            var nickname = nameof(Save_NullData) + GUID.Generate();
            var saveFile = new SaveFile<TestSaveData>(SaveFolder, nickname, bDay);

            AssertAll.Of(
                () => Assert.That(saveFile, Has.Property(nameof(saveFile.Data)).Null),
                () => Assert.That(saveFile, Has.Property(nameof(saveFile.File)).Not.Exist),
                () => Assert.That(saveFile, Has.Property(nameof(saveFile.Exists)).False),
                () => Validate.SaveFile_Basic(saveFile, nickname, bDay)
            );

            saveFile.Save(today);

            AssertAll.Of(
                () => Assert.That(saveFile, Has.Property(nameof(saveFile.Data)).Null),
                () => Assert.That(saveFile, Has.Property(nameof(saveFile.File)).Exist),
                () => Assert.That(saveFile, Has.Property(nameof(saveFile.Exists)).True),
                () => Validate.SaveFile_Basic(saveFile, nickname, today)
            );
        }
    }
}