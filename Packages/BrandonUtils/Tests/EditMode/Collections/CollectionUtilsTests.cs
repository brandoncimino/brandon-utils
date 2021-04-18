using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using BrandonUtils.Collections;
using BrandonUtils.Logging;

using Newtonsoft.Json;

using NUnit.Framework;

// ReSharper disable ReturnValueOfPureMethodIsNotUsed

namespace BrandonUtils.Tests.EditMode.Collections {
    [TestOf(typeof(CollectionUtils))]
    public class CollectionUtilsTests {
        private static Dictionary<int, string> ValidDictionary => new Dictionary<int, string> {
            {1, "one"},
            {2, "two"},
            {3, "three"}
        };

        private static Dictionary<string, int> ValidDictionary_Inverse => new Dictionary<string, int> {
            {"one", 1},
            {"two", 2},
            {"three", 3}
        };

        [Test]
        public void ValidDictionaryInverse() {
            Assert.That(ValidDictionary,           Is.Not.EqualTo(ValidDictionary_Inverse));
            Assert.That(ValidDictionary.Inverse(), Is.EqualTo(ValidDictionary_Inverse));
        }

        [Test]
        public void InvertInvalidDictionaryWithDuplicateValues() {
            var initialDictionary = new Dictionary<int, string> {
                {1, "one"},
                {2, "two"},
                {3, "two"}
            };

            Assert.Throws<ArgumentException>(() => initialDictionary.Inverse());
        }

        [Test]
        public void InvertInvalidDictionaryWithNullValues() {
            var initialDictionary = new Dictionary<int, string> {
                {1, "one"},
                {2, null},
                {3, "three"}
            };

            Assert.Throws<ArgumentNullException>(() => initialDictionary.Inverse());
        }

        [Test]
        public void InverseOfNonReadOnly() {
            var nonReadOnlyInverse = ValidDictionary.Inverse();
            Assert.That(nonReadOnlyInverse, Is.TypeOf<Dictionary<string, int>>());
            Assert.That(nonReadOnlyInverse, Is.Not.TypeOf<ReadOnlyDictionary<string, int>>());
        }

        [Test]
        public void InverseOfReadOnly() {
            var readOnly        = new ReadOnlyDictionary<int, string>(ValidDictionary);
            var readOnlyInverse = readOnly.Inverse();
            Assert.That(readOnlyInverse, Is.TypeOf<ReadOnlyDictionary<string, int>>());
            Assert.That(readOnlyInverse, Is.Not.TypeOf<Dictionary<string, int>>());
        }

        [Test]
        public void InverseInternal() {
            var inv = CollectionUtils.Inverse_Internal(ValidDictionary);
            PrintDictionary(inv);
            Assert.That(inv, Is.EqualTo(ValidDictionary_Inverse));
        }

        private static void PrintDictionary<TKey, TValue>(IDictionary<TKey, TValue> dictionary) {
            LogUtils.Log(JsonConvert.SerializeObject(dictionary, Formatting.Indented));
        }

        #region Dictionary Joining

        [Test]
        public void JoinWithoutOverlap() {
            var dic1 = new Dictionary<int, int> {
                {1, 1},
                {2, 1}
            };

            var dic2 = new Dictionary<int, int> {
                {3, 2},
                {4, 2}
            };

            var expected = new Dictionary<int, int> {
                {1, 1},
                {2, 1},
                {3, 2},
                {4, 2}
            };

            Assert.That(dic1.JoinDictionaries(dic2), Is.EqualTo(expected));
        }

        private static Dictionary<int, int> DicOrigin => new Dictionary<int, int> {
            {1, 1},
            {2, 1},
            {3, 1}
        };

        private static Dictionary<int, int> DicOverlap => new Dictionary<int, int> {
            {3, 2},
            {4, 2},
            {5, 2}
        };

        private static Dictionary<int, int> ResultFavoringOriginal => new Dictionary<int, int> {
            {1, 1},
            {2, 1},
            {3, 1},
            {4, 2},
            {5, 2}
        };

        private static Dictionary<int, int> ResultFavoringNew => new Dictionary<int, int> {
            {1, 1},
            {2, 1},
            {3, 2},
            {4, 2},
            {5, 2}
        };

        [Test]
        public void JoinPreferOriginal_Extension() {
            var jointDic = DicOrigin.JoinDictionaries(DicOverlap, CollectionUtils.ConflictResolution.FavorOriginal);
            LogUtils.Log(
                $"Original: {JsonConvert.SerializeObject(DicOrigin)}",
                $"Overlap:  {JsonConvert.SerializeObject(DicOverlap)}",
                $"Actual:   {JsonConvert.SerializeObject(jointDic)}",
                $"Expected: {JsonConvert.SerializeObject(ResultFavoringOriginal)}"
            );
            Assert.That(jointDic, Is.EqualTo(ResultFavoringOriginal));
        }

        [Test]
        public void JoinPreferOriginal_Collection() {
            var jointDic = CollectionUtils.JoinDictionaries(new[] {DicOrigin, DicOverlap}, CollectionUtils.ConflictResolution.FavorOriginal);
            Assert.That(jointDic, Is.EqualTo(ResultFavoringOriginal));
        }

        [Test]
        public void JoinPreferNew_Collection() {
            var jointDic = CollectionUtils.JoinDictionaries(new[] {DicOrigin, DicOverlap}, CollectionUtils.ConflictResolution.FavorNew);
            Assert.That(jointDic, Is.EqualTo(ResultFavoringNew));
        }

        [Test]
        public void JoinPreferNew_Extension() {
            var jointDic = DicOrigin.JoinDictionaries(DicOverlap, CollectionUtils.ConflictResolution.FavorNew);
            LogUtils.Log(
                $"Actual:   {JsonConvert.SerializeObject(jointDic)}",
                $"Expected: {JsonConvert.SerializeObject(ResultFavoringNew)}"
            );
            Assert.That(jointDic, Is.EqualTo(ResultFavoringNew));
        }

        [Test]
        public void JoinFailure_Extension() {
            Assert.Throws<ArgumentException>(() => DicOrigin.JoinDictionaries(DicOverlap, CollectionUtils.ConflictResolution.Fail));
        }

        [Test]
        public void JoinFailure_Collection() {
            Assert.Throws<ArgumentException>(() => CollectionUtils.JoinDictionaries(new[] {DicOrigin, DicOverlap}, CollectionUtils.ConflictResolution.Fail));
        }

        [TestCase(CollectionUtils.ConflictResolution.FavorNew)]
        [TestCase(CollectionUtils.ConflictResolution.FavorOriginal)]
        public void JoiningDoesNotModify_Extension(CollectionUtils.ConflictResolution conflictResolution) {
            //calling "new Dictionary<>" isn't strictly required but I'm including it because I keep confusing myself with whether these things like DicOrigin are properties or methods or auto-properties or whatever so this way it won't break when I inevitable change them
            var dicOrigin  = new Dictionary<int, int>(DicOrigin);
            var dicOverlap = new Dictionary<int, int>(DicOverlap);

            dicOrigin.JoinDictionaries(dicOverlap, conflictResolution);

            LogUtils.Log(
                $"{nameof(dicOrigin)}:  {dicOrigin.JoinString()}",
                $"{nameof(DicOrigin)}:  {DicOrigin.JoinString()}",
                $"{nameof(dicOverlap)}: {dicOverlap.JoinString()}",
                $"{nameof(DicOverlap)}: {DicOverlap.JoinString()}"
            );

            Assert.That(dicOrigin,  Is.EqualTo(DicOrigin));
            Assert.That(dicOverlap, Is.EqualTo(DicOverlap));
        }

        [TestCase(CollectionUtils.ConflictResolution.FavorNew)]
        [TestCase(CollectionUtils.ConflictResolution.FavorOriginal)]
        public void JoiningDoesNotModify_Collection(CollectionUtils.ConflictResolution conflictResolution) {
            //calling "new Dictionary<>" isn't strictly required but I'm including it because I keep confusing myself with whether these things like DicOrigin are properties or methods or auto-properties or whatever so this way it won't break when I inevitable change them
            var dicOrigin  = new Dictionary<int, int>(DicOrigin);
            var dicOverlap = new Dictionary<int, int>(DicOverlap);

            CollectionUtils.JoinDictionaries(new[] {dicOrigin, dicOverlap}, conflictResolution);

            LogUtils.Log(
                $"{nameof(dicOrigin)}:  {dicOrigin.JoinString()}",
                $"{nameof(DicOrigin)}:  {DicOrigin.JoinString()}",
                $"{nameof(dicOverlap)}: {dicOverlap.JoinString()}",
                $"{nameof(DicOverlap)}: {DicOverlap.JoinString()}"
            );

            Assert.That(dicOrigin,  Is.EqualTo(DicOrigin));
            Assert.That(dicOverlap, Is.EqualTo(DicOverlap));
        }

        #endregion
    }
}
