using System;

using BrandonUtils.Standalone.Exceptions;
using BrandonUtils.Standalone.Optional;
using BrandonUtils.Testing;

using NUnit.Framework;

namespace BrandonUtils.Tests.Standalone.Collections {
    public class FailableTests {
        #region Example methods

        private static int Fail() {
            throw new BrandonException("This method always fails.");
        }

        private const int Expected_Value   = 5;
        private const int Unexpected_Value = 10;

        private static int Succeed() {
            return Expected_Value;
        }

        #endregion

        private static class Validate {
            public static void FailedFailable<T>(Failable<T> failable) {
                AssertAll.Of(
                    () => Assert.That(failable, Has.Property(nameof(failable.HasValue)).False),
                    () => Assert.That(failable, Has.Property(nameof(failable.Failed)).True),
                    () => Assert.DoesNotThrow(() => Console.WriteLine(failable.Excuse)),
                    () => Assert.Throws<InvalidOperationException>(() => Console.WriteLine(failable.Value))
                );
            }

            public static void PassedFailable<T>(Failable<T> failable) {
                AssertAll.Of(
                    () => Assert.That(failable, Has.Property(nameof(failable.HasValue)).True),
                    () => Assert.That(failable, Has.Property(nameof(failable.Failed)).False),
                    () => Assert.DoesNotThrow(() => Console.WriteLine(failable.Value)),
                    () => Assert.Throws<InvalidOperationException>(() => Console.WriteLine(failable.Excuse))
                );
            }

            public static void Equality<T>(Failable<T> failable, Optional<T> optional, bool expectedEquality) {
                AssertAll.Of(
                    () => Assert.That(failable == optional,                  Is.EqualTo(expectedEquality), "failable == optional"),
                    () => Assert.That(optional == failable,                  Is.EqualTo(expectedEquality), "optional == failable"),
                    () => Assert.That(failable.Equals(optional),             Is.EqualTo(expectedEquality), "failable.Equals(optional)"),
                    () => Assert.That(optional.Equals(failable),             Is.EqualTo(expectedEquality), "optional.Equals(failable)"),
                    () => Assert.That(Optional.AreEqual(optional, failable), Is.EqualTo(expectedEquality), "Optional.AreEqual(optional, failable)"),
                    () => Assert.That(Optional.AreEqual(failable, optional), Is.EqualTo(expectedEquality), "Optional.AreEqual(failable, optional)")
                );
            }

            public static void Equality<T>(Failable<T> a, Failable<T> b, bool expectedEquality) {
                AssertAll.Of(
                    () => Assert.That(a == b,                  Is.EqualTo(expectedEquality), "a == b"),
                    () => Assert.That(b == a,                  Is.EqualTo(expectedEquality), "b == a"),
                    () => Assert.That(a.Equals(b),             Is.EqualTo(expectedEquality), "a.Equals(b)"),
                    () => Assert.That(b.Equals(a),             Is.EqualTo(expectedEquality), "b.Equals(a)"),
                    () => Assert.That(Optional.AreEqual(a, b), Is.EqualTo(expectedEquality), "Optional.AreEqual(a,b)"),
                    () => Assert.That(Optional.AreEqual(b, a), Is.EqualTo(expectedEquality), "Optional.AreEqual(b,a)")
                );
            }

            public static void Equality<T>(Failable<T> failable, T expectedValue, bool expectedEquality) {
                AssertAll.Of(
                    () => Assert.That(failable == expectedValue,                       Is.EqualTo(expectedEquality), "failable == expectedValue"),
                    () => Assert.That(expectedValue == failable,                       Is.EqualTo(expectedEquality), "expectedValue == failable"),
                    () => Assert.That(failable.Equals(expectedValue),                  Is.EqualTo(expectedEquality), "failable.Equals(expectedValue)"),
                    () => Assert.That(Optional.AreEqual(failable,      expectedValue), Is.EqualTo(expectedEquality), "Optional.AreEqual(failable, expectedValue)"),
                    () => Assert.That(Optional.AreEqual(expectedValue, failable),      Is.EqualTo(expectedEquality), "Optional.AreEqual(expectedValue, failable)")
                );
            }
        }

        [Test]
        public void FailedFailable_Lambda() {
            Validate.FailedFailable(Optional.Try(Fail));
        }

        [Test]
        public void FailedFailable_Func() {
            Func<int> func = Fail;
            Validate.FailedFailable(func.Try());
        }

        [Test]
        public void PassedFailable_Lambda() {
            Validate.PassedFailable(Optional.Try(Succeed));
        }

        [Test]
        public void PassedFailable_Func() {
            Func<int> func = Succeed;
            Validate.PassedFailable(func.Try());
        }

        [Test]
        public void SuccessfulFailableEquality() {
            var failable = Optional.Try(Succeed);
            var optional = Optional.Of(Expected_Value);
            AssertAll.Of(
                () => Validate.Equality(failable, optional,       true),
                () => Validate.Equality(failable, Expected_Value, true)
            );
        }

        [Test]
        public void SuccessfulEqualsSelf() {
            var failable = Optional.Try(Succeed);
            Validate.Equality(failable, failable, true);
        }

        [Test]
        public void SuccessfulEqualsDuplicate() {
            var a = Optional.Try(Succeed);
            var b = Optional.Try(Succeed);
            Validate.Equality(a, b, true);
        }

        [Test]
        public void SuccessfulFailableInequality() {
            var failable = Optional.Try(Succeed);
            var optional = Optional.Of(Unexpected_Value);
            AssertAll.Of(
                () => Validate.Equality(failable, optional,         false),
                () => Validate.Equality(failable, Unexpected_Value, false)
            );
        }

        [Test]
        public void FailedFailableInequality() {
            var failable = Optional.Try(Fail);
            var optional = Optional.Of(Expected_Value);
            AssertAll.Of(
                () => Validate.Equality(failable, optional,              false),
                () => Validate.Equality(failable, Expected_Value,        false),
                () => Validate.Equality(failable, Optional.Try(Succeed), false)
            );
        }

        [Test]
        public void FailedFailableEquality() {
            var failable = Optional.Try(Fail);
            var optional = new Optional<int>();
            AssertAll.Of(
                () => Validate.Equality(failable, optional,           true),
                () => Validate.Equality(failable, failable,           true),
                () => Validate.Equality(failable, Optional.Try(Fail), true)
            );
        }

        [Test]
        public void FailableDefault() {
            Failable<int> failable = default;
            AssertAll.Of(
                () => Validate.FailedFailable(failable),
                () => Assert.That(failable, Has.Property(nameof(failable.Excuse)).Null)
            );
        }
    }
}