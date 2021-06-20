using System;
using System.Collections.Generic;

using BrandonUtils.Standalone.Collections;
using BrandonUtils.Standalone.Optional;
using BrandonUtils.Testing;

using NUnit.Framework;

namespace BrandonUtils.Tests.Standalone.Collections {
    public class OptionalTests {
        [Test]
        public void EmptyEqualsEmpty() {
            var a = new Optional<int>();
            var b = new Optional<int>();

            AssertAll.Of(
                () => Assert.That(a,                          Is.EqualTo(b), "a equals b"),
                () => Assert.That(a,                          Is.EqualTo(new Optional<int>())),
                () => Assert.That(a.Equals(new string[] { }), Is.False, "a should not equal a different empty collection of the same item type"),
                // ReSharper disable once SuspiciousTypeConversion.Global
                () => Assert.That(a.Equals(new Optional<string>()), Is.False, $"a (with item type {a.ItemType()}) should not equal an {a.GetType().Name} with item type {typeof(string)}"),
                () => Assert.That(a == b,                           "a == b")
            );
        }

        [Test]
        public void EmptyEqualsDefault(
            [Values(typeof(int), typeof(string), typeof(object), typeof(DateTime), typeof(List<int>))]
            Type type
        ) {
            var optionalType = typeof(Optional<>);
            var genericType  = optionalType.MakeGenericType(type);
            var obj          = Activator.CreateInstance(genericType);

            Console.WriteLine(obj);
        }

        [Test]
        public void DefaultEqualsEmpty() {
            Optional<int> a = default;

            Console.WriteLine(a);

            AssertAll.Of(
                a,
                Has.Property(nameof(a.HasValue)).EqualTo(false),
                Is.EqualTo(new Optional<int>())
            );
        }

        [Test]
        public void OptionalEqualsUnboxed_String() {
            const string str = "yolo";
            var          a   = new Optional<string>(str);

            AssertAll.Of(
                () => Assert.That(a.Equals(str), "a.Equals(str)"),
                () => Assert.That(a,             Is.EqualTo(str)),
                () => Assert.That(a == str,      "a == str"),
                () => Assert.That(str == a,      "str == a"),
                () => Assert.That(str,           Is.EqualTo(a)),
                () => Assert.That(a.Equals(str), "a.Equals(str)")
            );
        }

        [Test]
        public void OptionalEqualsUnboxed_IntLong() {
            const int  i = 5;
            const long l = 5;

            var iOpt = Optional.Of(i);
            var lOpt = Optional.Of(l);

            AssertAll.Of(
                () => Assert.That(iOpt == i, "iOpt == i"),
                // () => Assert.That(iOpt == l, "iOpt == l"),
                () => Assert.That(lOpt == i, "lOpt == i"),
                () => Assert.That(lOpt == l, "lOpt == l"),
                () => Assert.That(i == iOpt, "i == iOpt"),
                () => Assert.That(i == lOpt, "i == lOpt"),
                // () => Assert.That(l == iOpt, "l == iOpt"),
                () => Assert.That(l == lOpt, "l == lOpt")
            );
        }

        [Test]
        public void ReturnBoxed() {
            Assert.That(Boxional(5), Is.TypeOf<Optional<int>>());
        }

        [Test]
        public void ReturnUnboxed() {
            Assert.That(Unboxional(Optional.Of(5)), Is.TypeOf<int>());
        }

        [Test]
        public void PassUnboxional() {
            Assert.That(Unboxional<int>(5), Is.TypeOf<int>());
        }

        private static Optional<T> Boxional<T>(T value) {
            return value;
        }

        private static T Unboxional<T>(Optional<T> optional) {
            return optional.GetValueOrDefault((T) default);
        }
    }
}