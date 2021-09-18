using System;
using System.Text.RegularExpressions;

using BrandonUtils.Standalone.Strings;
using BrandonUtils.Testing;

using NUnit.Framework;

using Is = NUnit.Framework.Is;

namespace BrandonUtils.Tests.Standalone.Strings {
    public class SubstringTests {
        [Test]
        [TestCase("abc",       "b",        "a",    "c")]
        [TestCase("aabbcc",    "b",        "aa",   "cc")]
        [TestCase("aabbcc",    "bc",       "aab",  "c")]
        [TestCase("yolo.swag", ".",        "yolo", "swag")]
        [TestCase("buttz",     null,       "",     "")]
        [TestCase("asdfasdf",  "",         "",     "")]
        [TestCase(null,        "abc",      "",     "")]
        [TestCase("",          "",         "",     "")]
        [TestCase(null,        null,       "",     "")]
        [TestCase("",          "yolo",     "",     "")]
        [TestCase("yolo",      "yoloswag", "",     "")]
        public void Substring_Simple(string original, string splitter, string expected_before, string expected_after) {
            AssertAll.Of(
                () => Assert.That(original.SubstringBefore(splitter), Is.EqualTo(expected_before)),
                () => Assert.That(original.SubstringAfter(splitter),  Is.EqualTo(expected_after))
            );
        }

        [Test]
        [TestCase("yolo.swag",        ".",    "",    "")]
        [TestCase("b00bz",            @"\d",  "b",   "bz")]
        [TestCase("one%-two~!~three", @"\W+", "one", "three")]
        public void Substring_Regex(string original, string pattern, string expected_before, string expected_after) {
            AssertAll.Of(
                () => Assert.That(original.SubstringBefore(new Regex(pattern)), Is.EqualTo(expected_before)),
                () => Assert.That(original.SubstringAfter(new Regex(pattern)),  Is.EqualTo(expected_after))
            );
        }

        [Test]
        public void regFlag() {
            var regexOps = RegexOptions.Compiled | RegexOptions.Multiline;
            Console.WriteLine($"{nameof(regexOps)} = {regexOps}");
            Console.WriteLine($"is rtl = {regexOps.HasFlag(RegexOptions.RightToLeft)}");
            var ro2 = regexOps | RegexOptions.RightToLeft;
            Console.WriteLine($"{nameof(ro2)} = {ro2}, {ro2.HasFlag(RegexOptions.RightToLeft)}");
            var ro3 = ro2 | RegexOptions.RightToLeft;
            Console.WriteLine($"{nameof(ro3)} = {ro3}, {ro3.HasFlag(RegexOptions.RightToLeft)}");
        }
    }
}