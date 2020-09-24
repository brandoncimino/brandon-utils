using System;
using System.Collections.Generic;

using NUnit.Framework;

using Packages.BrandonUtils.Runtime.Collections;

namespace Packages.BrandonUtils.Tests {
    [TestOf(typeof(CollectionUtils))]
    public class CollectionUtilsTests {
        [Test]
        public void InvertValidDictionary() {
            var initialDictionary = new Dictionary<int, string> {
                {1, "one"},
                {2, "two"},
                {3, "three"}
            };

            var expectedDictionary = new Dictionary<string, int> {
                {"one", 1},
                {"two", 2},
                {"three", 3}
            };

            Assert.That(initialDictionary,          Is.Not.EqualTo(expectedDictionary));
            Assert.That(initialDictionary.Invert(), Is.EqualTo(expectedDictionary));
        }

        [Test]
        public void InvertInvalidDictionaryWithDuplicateValues() {
            var initialDictionary = new Dictionary<int, string> {
                {1, "one"},
                {2, "two"},
                {3, "two"}
            };

            Assert.Throws<ArgumentException>(() => initialDictionary.Invert());
        }

        [Test]
        public void InvertInvalidDictionaryWithNullValues() {
            var initialDictionary = new Dictionary<int, string> {
                {1, "one"},
                {2, null},
                {3, "three"}
            };

            Assert.Throws<ArgumentNullException>(() => initialDictionary.Invert());
        }
    }
}