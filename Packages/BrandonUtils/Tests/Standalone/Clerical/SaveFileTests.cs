using System;
using System.IO;

using BrandonUtils.Standalone.Clerical.Saving;
using BrandonUtils.Testing;

using NUnit.Framework;

namespace BrandonUtils.Tests.Standalone.Clerical {
    public class SaveFileTests {
        private static readonly SaveFolder TestFolder = new SaveFolder(Path.GetTempPath(), nameof(SaveFileTests));

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
            var nickname = nameof(Save_NullData) + Guid.NewGuid();
            var saveFile = new SaveFile<TestSaveData>(TestFolder, nickname, bDay);

            AssertAll.Of(
                () => Assert.That(saveFile, Has.Property(nameof(saveFile.Data)).Null),
                () => Assert.That(saveFile, Has.Property(nameof(saveFile.File)).Not.Exist),
                () => Assert.That(saveFile, Has.Property(nameof(saveFile.Exists)).False),
                () => Validate.SaveFile_Basic(saveFile, nickname, bDay)
            );

            saveFile.Save();

            AssertAll.Of(
                () => Assert.That(saveFile, Has.Property(nameof(saveFile.Data)).Null),
                () => Assert.That(saveFile, Has.Property(nameof(saveFile.File)).Exist),
                () => Assert.That(saveFile, Has.Property(nameof(saveFile.Exists)).True),
                () => Validate.SaveFile_Basic(saveFile, nickname, bDay)
            );
        }

        private static ISaveSlot<TestSaveData> GetUniqueSlot() {
            return new SimpleSaveSlot<TestSaveData>(TestFolder, nameof(SimpleSaveSlot<TestSaveData>) + Guid.NewGuid());
        }

        [Test]
        public void NewSaveSlotIsEmpty() {
            var saveSlot = GetUniqueSlot();

            AssertAll.Of(
                saveSlot,
                Has.Property(nameof(saveSlot.SaveFileCount)).EqualTo(0),
                Has.Property(nameof(saveSlot.SaveFileCount)).EqualTo(0)
            );
        }
    }
}