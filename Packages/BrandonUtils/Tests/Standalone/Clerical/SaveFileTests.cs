using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

using BrandonUtils.Standalone.Clerical.Saving;
using BrandonUtils.Standalone.Strings;
using BrandonUtils.Testing;

using NUnit.Framework;

using Is = BrandonUtils.Testing.Is;

namespace BrandonUtils.Tests.Standalone.Clerical {
    [SuppressMessage("ReSharper", "AccessToStaticMemberViaDerivedType")]
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

            public static void SaveFile_Unloaded<T>(ISaveFile<T> saveFile) where T : ISaveData {
                Asserter.Against(saveFile)
                        .WithHeading($"{nameof(saveFile)} must exist but not be loaded")
                        .And(Has.Property(nameof(saveFile.Exists)).True)
                        .And(Has.Property(nameof(saveFile.Data)).Null)
                        .Invoke();
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

        [Test]
        public void Save_RealData() {
            var saveData = new TestSaveData(nameof(Save_RealData) + Guid.NewGuid());

            var saveFile = new SaveFile<TestSaveData>(TestFolder, nameof(Save_RealData) + "_File", DateTime.Now, default, saveData);

            Asserter.Against(saveFile)
                    .And(Has.Property(nameof(saveFile.Data)).Not.Null)
                    .And(Has.Property(nameof(saveFile.Exists)).False)
                    .Invoke();

            saveFile.Save();

            Asserter.Against(saveFile)
                    .And(Has.Property(nameof(saveFile.Data)).Not.Null)
                    .And(Has.Property(nameof(saveFile.Exists)).True)
                    .Invoke();
        }

        [Test]
        public void NewSaveFileFromExistingFile() {
            //create a file
            var saveData      = new TestSaveData(nameof(NewSaveFileFromExistingFile)                       + Guid.NewGuid());
            var firstSaveFile = new SaveFile<TestSaveData>(TestFolder, nameof(NewSaveFileFromExistingFile) + "_File", DateTime.Now, default, saveData);

            firstSaveFile.Save();

            var secondSaveFile = new SaveFile<TestSaveData>(TestFolder, firstSaveFile.File);

            Asserter.Against(secondSaveFile)
                    .Exists()
                    .And(Has.Property(nameof(secondSaveFile.Data)).Null)
                    .And(Has.Property(nameof(secondSaveFile.Nickname)).EqualTo(firstSaveFile.Nickname))
                    .And(Has.Property(nameof(secondSaveFile.TimeStamp)).EqualTo(firstSaveFile.TimeStamp))
                    .Invoke();

            secondSaveFile.Load();
            Asserter.Against(secondSaveFile)
                    .Exists()
                    .And(Has.Property(nameof(secondSaveFile.Data)).Not.Null)
                    .And(() => Assert.True(firstSaveFile.Data.Equals(secondSaveFile.Data), "firstSaveFile.Data.Equals(secondSaveFile.Data)"))
                    .And(Has.Property(nameof(secondSaveFile.Data)).EqualTo(firstSaveFile.Data))
                    .Invoke();
        }


    }
}