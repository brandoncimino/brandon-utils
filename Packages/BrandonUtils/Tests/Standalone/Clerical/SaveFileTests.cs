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

            public static void SaveFiles_AreEquivalent<T>(ISaveFile<T> first, ISaveFile<T> second) where T : ISaveData {
                Asserter.Against(first)
                        .WithHeading($"First {first.GetType().Prettify()} must be equivalent to the second {second.GetType().Prettify()}")
                        .And(Has.Property(nameof(first.Nickname)).EqualTo(second.Nickname))
                        .And(Has.Property(nameof(first.TimeStamp)).EqualTo(second.TimeStamp))
                        .And(Has.Property(nameof(first.Data)).EqualTo(second.Data))
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

        private static ISaveSlot<TestSaveData> GetUniqueSlot() {
            return new SimpleSaveSlot<TestSaveData>(TestFolder, nameof(SimpleSaveSlot<TestSaveData>) + Guid.NewGuid());
        }

        [Test]
        public void NewSaveSlotIsEmpty() {
            var saveSlot = GetUniqueSlot();
            Console.WriteLine(saveSlot.SaveFileCount);
            Assert.That(saveSlot, Has.Property(nameof(saveSlot.SaveFileCount)).EqualTo(0));
        }

        [Test]
        public void SaveSlotNewFile() {
            var saveSlot = GetUniqueSlot();
            var nickname = nameof(SaveSlotNewFile) + Guid.NewGuid();
            var data     = new TestSaveData(nickname);

            Assert.That(saveSlot, Has.Property(nameof(saveSlot.SaveFileCount)).EqualTo(0));

            // save to the slot for the first time
            var now       = DateTime.Today;
            var firstSave = saveSlot.Save(data, now);

            Asserter.WithHeading("After the first save")
                    .And(
                        Asserter.Against(saveSlot)
                                .And(Has.Property(nameof(saveSlot.SaveFileCount)).EqualTo(1))
                                .And(
                                    it => it.EnumerateSaveFiles().Select(f => f.TimeStamp),
                                    Contains.Item(now)
                                )
                    )
                    .And(
                        Asserter.Against(firstSave)
                                .Exists()
                                .Nicknamed(saveSlot.Nickname)
                                .TimeStamped(now)
                                .And(Has.Property(nameof(firstSave.Data)).EqualTo(data))
                    )
                    .And(
                        Asserter.Against(saveSlot.LatestFile()?.Load())
                                .And(it => Validate.SaveFiles_AreEquivalent(it, firstSave))
                    )
                    .Invoke();

            // save to the slot a _second_ time
            now += TimeSpan.FromDays(1);
            var secondSave = saveSlot.Save(data, now);
            Asserter.WithHeading("After second save")
                    .And(
                        Asserter.Against(saveSlot)
                                .And(Has.Property(nameof(saveSlot.SaveFileCount)).EqualTo(2))
                                .And(
                                    it => it.EnumerateSaveFiles().Select(f => f.TimeStamp),
                                    Contains.Item(now)
                                )
                    )
                    .And(
                        Asserter.Against(secondSave)
                                .Exists()
                                .Nicknamed(saveSlot.Nickname)
                                .TimeStamped(now)
                                .And(Has.Property(nameof(secondSave.Data)).EqualTo(data))
                                .And(Has.Property(nameof(secondSave.Data)).EqualTo(firstSave.Data))
                                .And(Has.Property(nameof(secondSave.TimeStamp)).Not.EqualTo(firstSave.TimeStamp))
                    )
                    .And(
                        Asserter.Against(saveSlot.LatestFile()?.Load())
                                .Exists()
                                .And(it => Validate.SaveFiles_AreEquivalent(it, secondSave))
                    )
                    .Invoke();
        }

        [Test]
        public void SaveToSlot() {
            var saveSlot = GetUniqueSlot();
            var nickname = nameof(SaveToSlot) + Guid.NewGuid();
            var data     = new TestSaveData(nickname);

            var startTime = DateTime.Today;

            for (int i = 0; i < 5; i++) {
                var now = startTime.AddDays(i);
                data.Counter = i;

                var saved  = saveSlot.Save(data, now);
                var latest = saveSlot.LatestFile();
                latest?.Load();

                // expectations
                Asserter.Against(saveSlot)
                        .WithHeading($"Save Slot iteration #{i}")
                        .And(Has.Property(nameof(saveSlot.SaveFileCount)).EqualTo(i + 1))
                        .And(
                            Asserter.Against(latest)
                                    .WithHeading($"Latest File #{i}")
                                    .And(Is.Not.Null)
                                    .Exists()
                                    .Nicknamed(saveSlot.Nickname)
                                    .TimeStamped(now)
                                    .And(Has.Property(nameof(latest.Data)).Not.Null)
                                    .And(it => it.Data?.Counter, Is.EqualTo(i))
                        )
                        .And(
                            Asserter.Against(saved)
                                    .WithHeading($"Returned file from {nameof(saveSlot)}.{nameof(saveSlot.Save)}")
                                    .And(Is.Not.Null)
                                    .Exists()
                                    .Nicknamed(saveSlot.Nickname)
                                    .TimeStamped(now)
                                    .And(Has.Property(nameof(saved.Data)).Not.Null)
                                    .And(it => it.Data?.Counter, Is.EqualTo(i))
                                    .And(it => it.Data,          Is.EqualTo(latest.Data))
                        )
                        .Invoke();
            }
        }
    }
}