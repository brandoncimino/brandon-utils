using System;

using BrandonUtils.Standalone.Clerical.Saving;
using BrandonUtils.Testing;

using NUnit.Framework;

namespace BrandonUtils.Tests.Standalone.Clerical {
    public static class SaveFileAsserter {
        public static Asserter<ISaveFile<T>> Exists<T>(this Asserter<ISaveFile<T>> self, bool expectedExistence = true) where T : ISaveData {
            self.And(Has.Property(nameof(SaveFile<T>.Exists)).EqualTo(expectedExistence));
            return self;
        }

        public static Asserter<SaveFile<T>> Exists<T>(this Asserter<SaveFile<T>> self, bool expectedExistence = true) where T : ISaveData {
            self.And(Has.Property(nameof(SaveFile<T>.Exists)).EqualTo(expectedExistence));
            return self;
        }

        public static Asserter<ISaveFile<T>> Nicknamed<T>(this Asserter<ISaveFile<T>> self, string expectedNickname) where T : ISaveData {
            self.And(Has.Property(nameof(ISaveFile<ISaveData>.Nickname)).EqualTo(expectedNickname));
            return self;
        }

        public static Asserter<ISaveFile<T>> TimeStamped<T>(this Asserter<ISaveFile<T>> self, DateTime expectedTimeStamp) where T : ISaveData {
            self.And(Has.Property(nameof(ISaveFile<ISaveData>.TimeStamp)).EqualTo(expectedTimeStamp));
            return self;
        }
    }
}