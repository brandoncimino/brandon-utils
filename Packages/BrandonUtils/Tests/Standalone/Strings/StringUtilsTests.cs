﻿using System;
using System.Collections.Generic;
using System.Linq;

using BrandonUtils.Standalone;
using BrandonUtils.Standalone.Collections;
using BrandonUtils.Standalone.Strings;
using BrandonUtils.Testing;

using NUnit.Framework;

using Is = NUnit.Framework.Is;

namespace BrandonUtils.Tests.Standalone.Strings {
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

        [Test]
        [TestCase(
            @"a1
b
b
a2
b
b
a3",
            @"a1
...(2/7 lines omitted)
a2
...(2/7 lines omitted)
a3"
        )]
        [TestCase(
            @"a1
b
b
b
b
b",
            @"a1
...(5/6 lines omitted)"
        )]
        [TestCase(
            @"b
b
a
b
b",
            @"...(2/5 lines omitted)
a
...(2/5 lines omitted)"
        )]
        [TestCase(
            @"b
a
b
a
b",
            @"...
a
...
a
..."
        )]
        public void CollapseLines(string lines, string expected) {
            var split     = lines.SplitLines();
            var collapsed = StringUtils.CollapseLines(split, s => s == "b");
            Console.WriteLine(collapsed.JoinLines());
            Assert.That(collapsed, Is.EqualTo(expected.SplitLines()));
        }

        [Test]
        public void TruncateLines(
            [Values(1, 5, 10, 50, 100)]
            int lineCount,
            [Values(1, 2, 10, 50, 100)]
            int truncateTo,
            [Values(true, false)]
            bool includeMessage
        ) {
            var ln            = Enumerable.Repeat("LINE", lineCount);
            var truncated     = ln.TruncateLines(truncateTo, includeMessage);
            var truncateCount = lineCount - truncateTo;

            if (lineCount > truncateTo) {
                AssertAll.Of(
                    () => Assert.That(truncated, Has.Length.EqualTo(truncateTo)),
                    () => {
                        if (includeMessage) {
                            AssertAll.Of(
                                () => Assert.That(truncated.Last(), Is.Not.EqualTo("LINE")),
                                () => Assert.That(truncated.Last(), Contains.Substring(truncateCount + ""))
                            );
                        }
                    }
                );
            }
            else if (lineCount <= truncateTo) {
                Assert.That(truncated, Is.EqualTo(ln));
            }
        }

        #region SplitLines

        public class SplitExpectation {
            public readonly string   InputString;
            public readonly string[] ExpectedLines;

            public SplitExpectation(string inputString, params string[] expectedLines) {
                this.InputString   = inputString;
                this.ExpectedLines = expectedLines;
            }
        }

        private static SplitExpectation[] SplitExpectations = new[] {
            new SplitExpectation("a\nb",       "a", "b"),
            new SplitExpectation("a\rb",       "a", "b"),
            new SplitExpectation("a\n\nb",     "a", "", "b"),
            new SplitExpectation("a\r\nb",     "a", "b"),
            new SplitExpectation("a\r\rb",     "a", "", "b"),
            new SplitExpectation("a\n\r\nb",   "a", "", "b"),
            new SplitExpectation("a\r\n\rb",   "a", "", "b"),
            new SplitExpectation("a\r\n\n\rb", "a", "", "", "b"),
            new SplitExpectation("a",          "a"),
            new SplitExpectation("a\n",        "a", ""),
            new SplitExpectation("\na",        "",  "a"),
            new SplitExpectation("",           ""),
            new SplitExpectation("\n",         "", ""),
            new SplitExpectation("\r\n\n",     "", "", "")
        };

        private static string[] FlatSplitInputs       => SplitExpectations.Select(it => it.InputString).ToArray();
        private static string[] FlatSplitExpectations => SplitExpectations.SelectMany(it => it.ExpectedLines).ToArray();

        [Test]
        public void SplitLines_SingleString([ValueSource(nameof(SplitExpectations))] SplitExpectation splitExpectation) {
            var split = splitExpectation.InputString.SplitLines();
            Assert.That(split, Is.EqualTo(splitExpectation.ExpectedLines));
        }

        [Test]
        public void SplitLines_Collection() {
            Assert.That(FlatSplitInputs.SplitLines(), Is.EqualTo(FlatSplitExpectations));
        }

        #endregion

        #region ToStringLines

        [TestCase(new object[] { 1, 2, 3 },    new[] { "1", "2", "3" })]
        [TestCase(new object[] { 1, null, 3 }, new[] { "1", "", "3" })]
        public void ToStringLines_Simple(object[] input, string[] expectedLines) {
            Assert.That(input.ToStringLines(), Is.EqualTo(expectedLines));
        }

        [Test]
        public void ToStringLines_SingleString([ValueSource(nameof(SplitExpectations))] SplitExpectation expectation) {
            Assert.That(expectation.InputString.ToStringLines(), Is.EqualTo(expectation.ExpectedLines));
        }

        [TestCase(new object[] { 1, 2, 3 },    "X", new[] { "1", "2", "3" })]
        [TestCase(new object[] { 1, null, 3 }, "X", new[] { "1", "X", "3" })]
        public void ToStringLines_WithNullPlaceholder(object[] input, string nullPlaceholder, string[] expectedLines) {
            Assert.That(input.ToStringLines(nullPlaceholder), Is.EqualTo(expectedLines));
        }

        [Test]
        public void ToStringLines_JaggedStringArray() {
            string[][] superJaggedInputs = {
                FlatSplitInputs,
                FlatSplitInputs,
            };

            var singleExpected = FlatSplitExpectations;
            var allExpected = singleExpected
                              .Concat(singleExpected)
                              .ToArray();
            var split = superJaggedInputs.ToStringLines();
            split.ForEach((it, i) => Console.WriteLine($"[{i}]{it}"));

            Assert.That(split, Is.EqualTo(allExpected));
        }

        [Test]
        public void ToStringLines_MixedTypes() {
            var inputs = FlatSplitExpectations;
            var str = new object[] {
                inputs,
                "yolo",
                inputs
            };

            var singleExpected = FlatSplitExpectations;
            var allExpected = singleExpected
                              .Append("yolo")
                              .Concat(singleExpected)
                              .ToArray();

            Assert.That(str.ToStringLines(), Is.EqualTo(allExpected));
        }

        [Test]
        [TestCase("abc")]
        public void ToStringLines_SingleLineString(string original) {
            var expected = new[] { original };
            var actual   = original.ToStringLines();
            Assert.That(actual, Is.EqualTo(expected));
        }

        private class ToStringOverride {
            public string Value;

            public override string ToString() {
                return $"[{Value}]";
            }
        }

        [Test]
        public void ToStringLines_WithToStringOverrides() {
            var jaggedArray = new object[] {
                "a",
                2,
                new ToStringOverride() { Value = "yolo" },
                new List<object>() {
                    new ToStringOverride() { Value = "first" },
                    new ToStringOverride() { Value = "second" },
                    new ToStringOverride() { Value = "third" },
                }
            };

            var expectedLines = new[] {
                "a",
                "2",
                "[yolo]",
                "[first]", "[second]", "[third]"
            };

            Assert.That(jaggedArray.ToStringLines(), Is.EqualTo(expectedLines));
        }

        #endregion

        #region Indent

        [Test]
        [TestCase("a",  1, 2, ' ', "  a")]
        [TestCase(" b", 3, 1, ' ', "    b")]
        public void IndentString(string original, int indentCount, int indentSize, char indentChar, string expectedString) {
            Assert.That(original.Indent(indentCount, indentSize, indentChar), Is.EqualTo(expectedString));
        }

        public class IndentExpectation {
            public IEnumerable<string> OriginalLines;
            public IEnumerable<string> ExpectedLines;
            public int                 IndentCount;
            public int                 IndentSize;
            public char                IndentChar;

            public IEnumerable<string> ActualLines => OriginalLines.Indent(IndentCount, IndentSize, IndentChar);
        }

        public static IEnumerable<IndentExpectation> GetIndentExpectations() {
            return new IndentExpectation[] {
                new IndentExpectation() {
                    OriginalLines = new[] {
                        "a",
                        "b",
                        "c",
                        " d"
                    },
                    ExpectedLines = new[] {
                        "  a",
                        "  b",
                        "  c",
                        "   d"
                    },
                    IndentChar  = StringUtils.DefaultIndentChar,
                    IndentCount = 1,
                    IndentSize  = StringUtils.DefaultIndentSize
                },
                new IndentExpectation() {
                    OriginalLines = new[] {
                        "  a",
                        "    b"
                    },
                    IndentChar  = '%',
                    IndentCount = 3,
                    IndentSize  = 4,
                    ExpectedLines = new[] {
                        "%%%%%%%%%%%%  a",
                        "%%%%%%%%%%%%    b"
                    }
                }
            };
        }

        [Test]
        public void IndentLines([ValueSource(nameof(GetIndentExpectations))] IndentExpectation expectation) {
            Assert.That(expectation.ActualLines, Is.EqualTo(expectation.ExpectedLines));
        }

        [Test]
        public void NestedIndent() {
            var t1 = 2.Repeat("t1");
            var t2 = 2.Repeat("t2");
            var actualLines = t1
                              .Indent(0)
                              .Concat(t2.Indent(1))
                              .Indent(1)
                              .ToArray();

            var expectedLines = new[] {
                "  t1",
                "  t1",
                "    t2",
                "    t2"
            };

            Console.WriteLine($"\n{nameof(actualLines)}\n{actualLines.JoinLines()}");
            Console.WriteLine($"\n{nameof(expectedLines)}\n{expectedLines.JoinLines()}");

            Assert.That(actualLines, Is.EqualTo(expectedLines));
        }

        [Test]
        public void IndentWithLabel() {
            var lines = new[] {
                "a",
                "b",
                "c",
                " d"
            };

            var label  = "YOLO";
            var joiner = " - ";

            var expectedLines = new[] {
                "YOLO - a",
                "       b",
                "       c",
                "        d"
            };


            var actualLines = lines.IndentWithLabel(label, joiner).ToArray();
            Console.WriteLine($"{nameof(actualLines)}\n{actualLines.JoinLines()}");
            Console.WriteLine($"{nameof(expectedLines)}\n{expectedLines.JoinLines()}");

            Assert.That(actualLines, Is.EqualTo(expectedLines));
        }

        [Test]
        public void FormatNUnitException() {
            var exc = GetNUnitFailure(8, 5);

            Console.WriteLine($"message:\n{exc.Message}\n");
            Console.WriteLine($"stack:\n{exc.StackTrace}\n");
        }

        private Exception GetNUnitFailure(object actual, object expected) {
            try {
                Assert.That(actual, Is.EqualTo(expected));
                return null;
            }
            catch (Exception e) {
                return e;
            }
        }

        #endregion

        #region Trim

        [Test]
        [TestCase("KEEPabc",     "abc", "KEEP")]
        [TestCase("bcONEbcbc",   "bc",  "bcONE")]
        [TestCase("bTWObbcbc",   "bc",  "bTWOb")]
        [TestCase("a..........", ".",   "a")]
        public void TrimEnd(string input, string trimString, string expected) {
            Assert.That(input.TrimEnd(trimString), Is.EqualTo(expected));
        }

        [Test]
        [TestCase("abcKEEP",     "abc", "KEEP")]
        [TestCase("bcbcONEbcbc", "bc",  "ONEbcbc")]
        [TestCase("5.[t]JK",     "5.",  "[t]JK")]
        [TestCase("\\.\\.!!",    "\\.", "!!")]
        public void TrimStart(string input, string trimString, string expected) {
            Assert.That(input.TrimStart(trimString), Is.EqualTo(expected));
        }

        [Test]
        [TestCase("--a--",                 "-",    "a")]
        [TestCase("``jk`",                 "`",    "jk")]
        [TestCase("#yolo",                 "yolo", "#")]
        [TestCase("abc.abc.abc.!abc.abc.", "abc.", "!")]
        public void Trim(string input, string trimString, string expected) {
            Assert.That(input.Trim(trimString), Is.EqualTo(expected));
        }

        #endregion
    }
}