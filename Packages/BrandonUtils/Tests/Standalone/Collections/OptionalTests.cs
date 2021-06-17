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

            TestUtils.AssertAll(
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

            TestUtils.AssertAll(
                a,
                Has.Property(nameof(a.HasValue)).EqualTo(false),
                Is.EqualTo(new Optional<int>())
            );
        }

        [Test]
        public void OptionalEqualsUnboxed() {
            var str = "yolo";
            var a   = new Optional<string>(str);

            TestUtils.AssertAll(
                () => Assert.That(a.Equals(str), Is.True),
                () => Assert.That(a,             Is.EqualTo(str))
            );
        }

        [Test]
        public void OptionalFloatEqualsInt() {
            var a = new Optional<float>(10);
            var b = new Optional<int>(10);

            TestUtils.AssertAll(
                () => Assert.That(a, Is.EqualTo(b))
                // () => Assert.That(a == b, Is.True)
            );
        }
    }
}