using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using BrandonUtils.Standalone.Collections;
using BrandonUtils.Standalone.Strings;
using BrandonUtils.Standalone.Strings.Prettifiers;
using BrandonUtils.Testing;

using NUnit.Framework;

using Is = NUnit.Framework.Is;

namespace BrandonUtils.Tests.Standalone.Strings {
    public class PrettificationTests {
        public class Expectation {
            public object                 Original;
            public string                 Expected;
            public PrettificationSettings Settings;
        }

        private class HasToStringOverride {
            public DayOfWeek Value;

            public override string ToString() {
                return $"My {nameof(DayOfWeek)} is #{(int)Value}: {Value}";
            }
        }

        [Test]
        [TestCase(typeof(int), nameof(Int32))]
        [TestCase(
            typeof(KeyValuePair<DayOfWeek, string>),
            "KeyValuePair<DayOfWeek, String>"
        )]
        [TestCase(
            typeof(List<KeyValuePair<Collection<short>, uint>>),
            "List<KeyValuePair<Collection<Int16>, UInt32>>"
        )]
        public void PrettifyType(Type actualType, string expectedString) {
            Console.WriteLine(
                new[] {
                    $"ToString: {actualType}",
                    $"Name:     {actualType.Name}",
                    $"FullName: {actualType.FullName}",
                    $"Prettify: {actualType.Prettify()}",
                    $"DeclaringType:    {actualType.DeclaringType}"
                }.JoinLines()
            );
            Assert.That(actualType.Prettify(), Is.EqualTo(expectedString));
        }

        [Test]
        public void PrettifyFallsBackToToString() {
            var obj = new HasToStringOverride() { Value = DayOfWeek.Friday };

            Assert.That(
                obj.Prettify(),
                Is.EqualTo(obj.ToString())
            );
        }

        #region Collections

        #region Dictionaries

        [Test]
        public void PrettifyDictionary() {
            var dic = new Dictionary<string, DayOfWeek> { { "one", DayOfWeek.Monday }, { "two", DayOfWeek.Tuesday }, { "three", DayOfWeek.Wednesday } };

            var expectedString = @"
String DayOfWeek
------ ---------
one    Monday   
two    Tuesday  
three  Wednesday";

            Assert.That(dic.Prettify(), Is.EqualTo(expectedString.TrimStart()));
        }

        private static Expectation[] Tuple2Expectations = new[] { new Expectation() { Original = ("one", 1), Expected = "(one, 1)" }, new Expectation() { Original = (new List<double>() { 1, 99, double.PositiveInfinity }, ("yolo", 80085)), Expected = $"([1, 99, {double.PositiveInfinity}], (yolo, 80085))" }, new Expectation() { Original = (double.PositiveInfinity, "beyond"), Expected = $"({double.PositiveInfinity}, beyond)" } };

        [Test]
        public void PrettifyTuple2([ValueSource(nameof(Tuple2Expectations))] Expectation expectation) {
            var actualString = expectation.Original.Prettify();
            Console.WriteLine(actualString);
            Assert.That(actualString, Is.EqualTo(expectation.Expected.Trim()));
        }

        [Test]
        public void PrettifyKeyedList() {
            var prettifier = new Prettifier<KeyedList<object, object>>(InnerPretty.PrettifyKeyedList);

            var keyedList = new KeyedList<int, (int, string)>(it => it.Item1) { (1, "one"), (2, "two"), (99, "ninety-nine") };

            var expectedString = $@"
Int32 (Int32, String)  
----- -----------------
1     (1, one)         
2     (2, two)         
99    (99, ninety-nine)";
            var actualPrettifiedString = prettifier.Prettify(keyedList);
            Console.WriteLine(actualPrettifiedString);
            Assert.That(actualPrettifiedString.Trim(), Is.EqualTo(expectedString.Trim()));
        }

        [Test]
        public void FindGenericallyTypedPrettifier() {
            var keyedList = new KeyedList<int, (int, string)>(it => it.Item1) { (1, "one"), (2, "two"), (99, "ninety-nine") };

            var genTypeDef = keyedList.GetType().GetGenericTypeDefinition();
            Console.WriteLine($"gen def ass from real: {genTypeDef.IsInstanceOfType(keyedList)}");
            Console.WriteLine($"real ass from gen def: {keyedList.GetType().IsAssignableFrom(genTypeDef)}");

            var enumerableTypeDef = typeof(IEnumerable<int>).GetGenericTypeDefinition();
            Console.WriteLine($"kl def ass from ie def: {genTypeDef.IsAssignableFrom(enumerableTypeDef)}");
            Console.WriteLine($"ie def ass from kl def: {enumerableTypeDef.IsAssignableFrom(genTypeDef)}");

            var prettifier = Prettification.FindPrettifier(keyedList.GetType());

            Console.WriteLine(prettifier);
            Console.WriteLine(prettifier.Select(it => it.Prettify(keyedList)));
        }

        #endregion

        #endregion

        #region Additionally Registered Prettifiers

        class A {
            public override string ToString() {
                return $"{GetType().Name}.{nameof(ToString)}";
            }
        }

        class B : A { }

        class C : B { }

        class D : C { }

        [SetUp]
        public void RegisterABC() {
            Prettification.RegisterPrettifier(new Prettifier<A>(a => $"A:pretty"));
            Prettification.RegisterPrettifier(new Prettifier<B>(b => $"B:pretty"));
            Prettification.RegisterPrettifier(new Prettifier<C>(c => $"C:pretty"));
        }

        [TearDown]
        public void UnregisterABC() {
            Prettification.UnregisterPrettifier(typeof(A));
            Prettification.UnregisterPrettifier(typeof(B));
            Prettification.UnregisterPrettifier(typeof(C));
        }

        [Test]
        [TestCase(typeof(A), "A:pretty")]
        [TestCase(typeof(B), "B:pretty")]
        [TestCase(typeof(C), "C:pretty")]
        [TestCase(typeof(D), "A:pretty")]
        public void PrettifierPrefersExactType(Type actualType, string expectedString) {
            var prettifier = Prettification.FindPrettifier(actualType);
            Console.WriteLine(prettifier);

            var instance = Convert.ChangeType(actualType.GetConstructor(Array.Empty<Type>())?.Invoke(Array.Empty<object>()), actualType);
            var s1       = prettifier.Value.Prettify(instance);
            Console.WriteLine($"s1: {s1}");
            var str = instance.Prettify();
            Assert.That(str, Is.EqualTo(expectedString));
        }

        [Test]
        public void PrettyPreference_A() {
            var a        = new A();
            var expected = "A:pretty";
            Console.WriteLine(a.Prettify());
            AssertAll.Of(
                () => Assert.That(a.Prettify(), Is.EqualTo(expected)),
                () => TestAsObject(a, expected)
            );
        }

        private static void TestAsObject(object obj, string expectedPrettyString) {
            var str = obj.Prettify();
            Console.WriteLine($"as obj: {str}");
            Assert.That(str, Is.EqualTo(expectedPrettyString));
        }

        [Test]
        public void PrettyPreference_B() {
            var b        = new B();
            var expected = "B:pretty";
            Console.WriteLine(b.Prettify());
            AssertAll.Of(
                () => Assert.That(b.Prettify(), Is.EqualTo(expected)),
                () => TestAsObject(b, expected)
            );
        }

        [Test]
        public void PrettyPreference_C() {
            var c        = new C();
            var expected = "C:pretty";
            Console.WriteLine(c.Prettify());
            Assert.That(c.Prettify(), Is.EqualTo(expected));
        }

        [Test]
        public void PrettyPreference_D() {
            var d        = new D();
            var expected = "A:pretty";
            Console.WriteLine(d.Prettify());
            Assert.That(d.Prettify(), Is.EqualTo(expected));
        }

        #endregion

        private static Expectation[] EnumerableExpectations() {
            var ls = new List<int>() { 1, 2, 3 };
            return new[] {
                new Expectation() { Original = ls, Expected = "[1, 2, 3]" },
                new Expectation() { Original = ls, Settings = PrettificationFlags.IncludeTypeLabels, Expected = "List<Int32>[1, 2, 3]" },
                new Expectation() {
                    Original = ls,
                    Settings = new PrettificationSettings() { PreferredLineStyle = PrettificationSettings.LineStyle.Multi, Flags = PrettificationFlags.IncludeTypeLabels },
                    Expected = @"
List<Int32>[
  1
  2
  3
]"
                }
            };
        }

        [Test]
        public void PrettifyEnumerable([ValueSource(nameof(EnumerableExpectations))] Expectation expectation) {
            var actual = expectation.Original.Prettify(expectation.Settings);
            Console.WriteLine(actual);
            Assert.That(actual, Is.EqualTo(expectation.Expected.Trim()));
        }

        [Test]
        public void GenTypesMatch() {
            var ls         = new List<int> { 1, 2, 3 };
            var lst        = ls.GetType();
            var typesMatch = Prettification.GenericTypesMatch(lst, typeof(IEnumerable<object>));
            Console.WriteLine(typesMatch);
        }

        #region Sketchbook

        #endregion
    }
}