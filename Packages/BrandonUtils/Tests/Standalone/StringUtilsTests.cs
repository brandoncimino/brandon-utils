using System;

using BrandonUtils.Standalone.Strings;
using BrandonUtils.Testing;

using NUnit.Framework;

namespace BrandonUtils.Tests.Standalone {
    public class StringUtilsTests {
        [Test]
        [TestCase(
            "",
            11,
            "[__]",
            "[__][__][__"
        )]
        [TestCase(
            "yolo",
            6,
            "123",
            "yolo12"
        )]
        [TestCase(
            "yolo",
            3,
            "123",
            "yolo"
        )]
        public void FillRight(string original, int desiredLength, string filler, string expectedResult) {
            Assert.That(original.FillRight(desiredLength, filler), Is.EqualTo(expectedResult));
        }

        [TestCase("abc",  2, "ab")]
        [TestCase("abc",  4, "abca")]
        [TestCase(" ",    6, "      ")]
        [TestCase("yolo", 9, "yoloyoloy")]
        [TestCase("yolo", 0, "")]
        public void Fill(string filler, int desiredLength, string expectedResult) {
            Assert.That(filler.Fill(desiredLength), Is.EqualTo(expectedResult));
        }

        [Test]
        public void Fill_EmptyFiller(
            [Values(0, 1, 2, 3)]
            int desiredLength
        ) {
            const string str = "yolo";
            AssertAll.Of(
                () => Assert.Throws<ArgumentException>(() => str.FillRight(desiredLength, "")),
                () => Assert.Throws<ArgumentException>(() => str.FillLeft(desiredLength, "")),
                () => Assert.Throws<ArgumentException>(() => "".Fill(desiredLength))
            );
        }

        [Test]
        public void Fill_NullFiller(
            [Values(0, 1, 2, 3)]
            int desiredLength
        ) {
            AssertAll.Of(
                () => Assert.Throws<ArgumentNullException>(() => "".FillRight(desiredLength, null)),
                () => Assert.Throws<ArgumentNullException>(() => "".FillLeft(desiredLength, null)),
                () => Assert.Throws<ArgumentNullException>(() => StringUtils.Fill(null, desiredLength))
            );
        }

        [Test]
        public void Fill_NegativeLength() {
            AssertAll.Of(
                () => Assert.Throws<ArgumentOutOfRangeException>(() => "a".FillRight(-1, "a")),
                () => Assert.Throws<ArgumentOutOfRangeException>(() => "a".FillLeft(-1, "a")),
                () => Assert.Throws<ArgumentOutOfRangeException>(() => "a".Fill(-1))
            );
        }

        [Test]
        [TestCase(
            "yolo",
            "[0]",
            "--",
            "[0][0][0][0][0\n" +
            "[0]--yolo--[0]\n" +
            "[0][0][0][0][0"
        )]
        [TestCase(
            "",
            "/",
            "0-",
            "//////\n" +
            "/0-0-/\n" +
            "//////"
        )]
        [TestCase(
            "Reasonable Heading",
            "=",
            " ",
            "======================\n" +
            "= Reasonable Heading =\n" +
            "======================"
        )]
        public void FormatHeader(string heading, string border, string padding, string expectedResult) {
            Assert.That(StringUtils.FormatHeading(heading, border, padding), Is.EqualTo(expectedResult));
        }
    }
}