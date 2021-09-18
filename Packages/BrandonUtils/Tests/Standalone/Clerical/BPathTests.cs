using System;
using System.IO;

using BrandonUtils.Standalone.Clerical;
using BrandonUtils.Standalone.Collections;
using BrandonUtils.Standalone.Optional;
using BrandonUtils.Standalone.Strings;
using BrandonUtils.Testing;

using NUnit.Framework;

using Is = NUnit.Framework.Is;

namespace BrandonUtils.Tests.Standalone.Clerical {
    public class BPathTests {
        [Test]
        [TestCase(".json",            "",     ".json")]
        [TestCase("yolo.swag",        "yolo", ".swag")]
        [TestCase("yo.lo.swag.ins",   "yo",   ".lo", ".swag", ".ins")]
        [TestCase("abc",              "abc")]
        [TestCase("a/b.c/.d.e",       "", ".d", ".e")]
        [TestCase("a/b.c/d",          "d")]
        [TestCase(@"a//b\\c../\d..e", "d", ".e")]
        public void GetExtensions(string path, string expectedFileName, params string[] expectedExtensions) {
            AssertAll.Of(
                () => Console.WriteLine($"{nameof(GetExtensions)}: {BPath.GetExtensions(path).Prettify()}"),
                () => Assert.That(BPath.GetExtensions(path),                Is.EqualTo(expectedExtensions)),
                () => Assert.That(BPath.GetFullExtension(path),             Is.EqualTo(expectedExtensions.JoinString())),
                () => Assert.That(BPath.GetFileNameWithoutExtensions(path), Is.EqualTo(expectedFileName)),
                () => AssertAll.Of(
                    $"{nameof(FileSystemInfo)} extensions",
                    () => Console.WriteLine($"{nameof(FileInfo)}: {new FileInfo(path)}, {new FileInfo(path).Name}, {new FileInfo(path).FullName}"),
                    () => Assert.That(new FileInfo(path).Extensions(),                Is.EqualTo(expectedExtensions)),
                    () => Assert.That(new FileInfo(path).FullExtension(),             Is.EqualTo(expectedExtensions.JoinString())),
                    () => Assert.That(new FileInfo(path).FileNameWithoutExtensions(), Is.EqualTo(expectedFileName))
                )
            );
        }

        public enum Should {
            Pass,
            Fail
        }

        [TestCase("this is really really really really really really really really really really really really really really really really really really really really really really really really really really really really really really really really really really really really really really really really really really really really really really really really long", Should.Fail, Should.Fail)]
        [Test]
        [TestCase("abc",          Should.Pass, Should.Pass)]
        [TestCase(".ssh",         Should.Pass, Should.Pass)]
        [TestCase("a|b",          Should.Fail, Should.Fail)]
        [TestCase("%$@#%!@:$#%[", Should.Fail, Should.Fail)]
        [TestCase(null,           Should.Fail, Should.Fail)]
        [TestCase("",             Should.Fail, Should.Fail)]
        [TestCase("\n",           Should.Fail, Should.Fail)]
        [TestCase("C:/",          Should.Pass, Should.Fail)]
        [TestCase("C:D:E",        Should.Fail, Should.Fail)]
        // [TestCase("//yolo",       Should.Pass, Should.Fail)]
        // [TestCase(@"\\yolo",      Should.Pass, Should.Fail)]
        // [TestCase("a/b",          Should.Pass, Should.Fail)]
        [TestCase(@"a\b",  Should.Pass, Should.Fail)]
        [TestCase(@":\\c", Should.Fail, Should.Fail)]
        public void IsValidFilename(string path, Should pathShould, Should fileNameShould) {
            AssertAll.Of(
                () => AssertAll.Of(
                    "Path Validation",
                    () => Assert.That(BPath.IsValidPath(path),  Is.EqualTo(pathShould                                    == Should.Pass)),
                    () => Assert.That(BPath.ValidatePath(path), Has.Property(nameof(Failable.Failed)).EqualTo(pathShould == Should.Fail))
                ),
                () => AssertAll.Of(
                    "FileName Validation",
                    () => Assert.That(BPath.IsValidFileName(path),  Is.EqualTo(fileNameShould                                    == Should.Pass)),
                    () => Assert.That(BPath.ValidateFileName(path), Has.Property(nameof(Failable.Failed)).EqualTo(fileNameShould == Should.Fail))
                )
            );
        }
    }
}