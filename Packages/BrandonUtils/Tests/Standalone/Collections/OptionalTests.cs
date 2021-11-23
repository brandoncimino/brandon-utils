using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using BrandonUtils.Standalone.Collections;
using BrandonUtils.Standalone.Optional;
using BrandonUtils.Standalone.Strings;
using BrandonUtils.Testing;

using Newtonsoft.Json;

using NUnit.Framework;

using Is = BrandonUtils.Testing.Is;

namespace BrandonUtils.Tests.Standalone.Collections {
    [SuppressMessage("ReSharper", "AccessToStaticMemberViaDerivedType")]
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
                () => Assert.That(a   == str,    "a == str"),
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
                () => Assert.That(lOpt == i,    "lOpt == i"),
                () => Assert.That(lOpt == l,    "lOpt == l"),
                () => Assert.That(i    == iOpt, "i == iOpt"),
                () => Assert.That(i    == lOpt, "i == lOpt"),
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
            return optional.GetValueOrDefault((T)default);
        }

        [Test]
        public void NullInterfaceEquality() {
            var               optional_a  = (IOptional<string>)null;
            var               optional_b  = (IOptional<string>)null;
            var               optional_c  = (Optional<string>)null;
            IOptional<string> def_a       = default;
            IOptional<string> def_b       = default;
            Optional<string>  def_c       = default;
            const string      null_string = default;
            AssertAll.Of(
                () => Assert.False(Optional.AreEqual(optional_a,  null_string), "Optional.AreEqual(optional_a, null_string)"),
                () => Assert.False(Optional.AreEqual(null_string, optional_a),  "Optional.AreEqual(null_string, optional_a)"),
                () => Assert.True(Optional.AreEqual(optional_a,   optional_b), "Optional.AreEqual(optional_a, optional_b)"),
                () => Assert.True(Optional.AreEqual(optional_b,   optional_a), "Optional.AreEqual(optional_b,optional_a)"),
                () => Assert.True(Optional.AreEqual(def_a,        def_b),      "Optional.AreEqual(def_a, def_b)"),
                () => Assert.True(Optional.AreEqual(def_a,        def_a),      "Optional.AreEqual(def_a, def_a)"),
                () => Assert.False(Optional.AreEqual(optional_a,  optional_c), "Optional.AreEqual(optional_a, optional_c)"),
                () => Assert.False(Optional.AreEqual(optional_c,  def_c),      "Optional.AreEqual(optional_c, def_c)")
            );
        }

        #region Optional of null

        [Test]
        public void OptionalOfNull_HasValue() {
            var ofNull = new Optional<string>(null);

            AssertAll.Of(
                ofNull,
                Has.Property(nameof(ofNull.HasValue)).True,
                Has.Property(nameof(ofNull.Value)).Null
            );
        }

        [Test]
        [SuppressMessage("ReSharper", "SuggestVarOrType_Elsewhere")]
        [SuppressMessage("ReSharper", "ExpressionIsAlwaysNull")]
        public void OptionalOfNull_Equality() {
            Optional<string>  ofNull           = new Optional<string>(null);
            Optional<string>  ofNull2          = new Optional<string>(null);
            string            nullValue        = (string)null;
            Optional<string>  nullOptional     = (Optional<string>)null;
            IOptional<string> nullInterface    = (IOptional<string>)null;
            Optional<string>  defaultOptional  = default;
            IOptional<string> defaultInterface = default;
            Optional<string>  empty            = new Optional<string>();

            Console.WriteLine($"ofNull: [{ofNull}]");
            Console.WriteLine($"nullOptional: [{nullOptional}]");
            Console.WriteLine($"defaultOptional: [{defaultOptional}]");

            AssertAll.Of(
                // vs. nullValue
                () => Assert.That(ofNull                == nullValue,             "ofNull == nullValue"),
                () => Assert.That(ofNull                != nullValue,             Is.False, "ofNull != nullValue"),
                () => Assert.That((ofNull == nullValue) == (nullValue == ofNull), "(ofNull == nullValue) == (nullValue == ofNull)"),
                () => Assert.That((ofNull != nullValue) == (nullValue != ofNull), "(ofNull != nullValue) == (nullValue != ofNull)"),
                // vs. nullOptional
                () => Assert.That(ofNull                   == nullOptional,             "ofNull == nullOptional"),
                () => Assert.That(ofNull                   != nullOptional,             Is.False, "ofNull != nullOptional"),
                () => Assert.That((ofNull == nullOptional) == (nullOptional == ofNull), "(ofNull == nullOptional) == (nullOptional == ofNull)"),
                () => Assert.That((ofNull != nullOptional) == (nullOptional != ofNull), "(ofNull != nullOptional) == (nullOptional != ofNull)"),
                // vs. nullInterface
                () => Assert.That(ofNull == nullInterface, Is.False, "ofNull == nullInterface"),
                () => Assert.That(ofNull != nullInterface, "ofNull != nullInterface"),
                // operators with a left-hand IOptional aren't supported as it would cause lots of ambiguity with the other operators
                // () => Assert.That((ofNull == nullInterface) == (nullInterface == ofNull), "(ofNull == nullInterface) == (nullInterface == ofNull)"),
                // vs. ofNull2
                () => Assert.That(ofNull == ofNull2, "ofNull == ofNull2"),
                () => Assert.That(ofNull != ofNull2, Is.False, "ofNull != ofNull2"),
                // vs. defaultOptional
                () => Assert.That(ofNull == defaultOptional,                                  Is.False, "ofNull == defaultOptional"),
                () => Assert.That(ofNull != defaultOptional,                                  "ofNull != defaultOptional"),
                () => Assert.That(ofNull,                                                     Is.Not.EqualTo(defaultOptional), "ofNull, Is.Not.EqualTo(defaultOptional)"),
                () => Assert.That((ofNull == defaultOptional) == (defaultOptional == ofNull), "(ofNull == defaultOptional) == (defaultOptional == ofNull)"),
                // vs. defaultInterface
                () => Assert.That(ofNull == defaultInterface, Is.False, "ofNull == defaultInterface"),
                () => Assert.That(ofNull != defaultInterface, "ofNull != defaultInterface"),
                () => Assert.That(ofNull,                     Is.Not.EqualTo(defaultInterface), "ofNull, Is.Not.EqualTo(defaultInterface)"),
                // vs. empty
                () => Assert.That(ofNull == empty,                        Is.False, "ofNull == empty"),
                () => Assert.That(ofNull != empty,                        "ofNull != empty"),
                () => Assert.That(ofNull,                                 Is.Not.EqualTo(empty), "ofNull, Is.Not.EqualTo(empty)"),
                () => Assert.That((ofNull == empty) == (empty == ofNull), "(ofNull == empty) == (empty == ofNull)")
            );
        }

        #endregion

        #region ToOptional

        [Test]
        public void ToOptional_MultipleItems() {
            var ls = new[] { 1, 2, 3 };

            Assert.Throws<InvalidOperationException>(() => ls.ToOptional());
        }

        [Test]
        public void ToOptional_Empty() {
            var ls = Array.Empty<int>();
            Asserter.Against(ls.ToOptional)
                    .And(Has.Property(nameof(IOptional<object>.Count)).EqualTo(0))
                    .And(Has.Property(nameof(IOptional<object>.HasValue)).False)
                    .And(it => it.Value, Throws.InvalidOperationException)
                    .And(Is.Empty)
                    .Invoke();
        }

        [Test]
        [TestCase(double.PositiveInfinity)]
        [TestCase("#yolo")]
        [TestCase(new[] { 1, 2, 3 })]
        [TestCase(default(object))]
        public void ToOptional_Single(object value) {
            // var value = double.NegativeInfinity;
            var ls = new object[] { value };
            Asserter.Against(ls.ToOptional)
                    .And(Has.Property(nameof(IOptional<object>.Count)).EqualTo(1))
                    .And(Has.Property(nameof(IOptional<object>.HasValue)).True)
                    .And(Has.Property(nameof(IOptional<object>.Value)).EqualTo(value))
                    .And(Is.Not.Empty)
                    .Invoke();
        }

        #endregion

        #region ToString

        public static (Optional<object>, string)[] GetOptionalToStringExpectations() {
            return new[] {
                (new Optional<object>(5), "Optional<object>[5]"),
                (new Optional<object>(), $"Optional<object>[{Optional.EmptyPlaceholder}]"),
                (new Optional<object>(null), $"Optional<object>[{new PrettificationSettings().NullPlaceholder.Value}]"),
                (new Optional<object>(new Optional<object>(new Optional<object>("yolo"))), "Optional<object>[Optional<object>[Optional<object>[yolo]]]")
            };
        }

        [Test]
        public void OptionalToString([ValueSource(nameof(GetOptionalToStringExpectations))] (Optional<object>, string) expectation) {
            var (optional, expectedString) = expectation;
            Assert.That(optional.ToString(), Is.EqualTo(expectedString));
        }

        #endregion

        #region Serialization

        [Test]
        public void SerializeOptional() {
            var optional = Optional.Of(5);
            var json     = JsonConvert.SerializeObject(optional);
            Console.WriteLine($"json: {json}");
            Assert.That(json, Is.EqualTo("[5]"));
        }

        [Test]
        public void SerializeEmptyOptional() {
            var optional = Optional.Empty<string>();
            var json     = JsonConvert.SerializeObject(optional);
            Console.WriteLine($"json: {json}");
            Assert.That(json, Is.EqualTo("[]"));
        }

        [Test]
        public void SerializeOptionalOfNull() {
            var optional = Optional.Of<string>(null);
            var json     = JsonConvert.SerializeObject(optional);
            Console.WriteLine($"json: {json}");
            Assert.That(json, Is.EqualTo("[null]"));
        }

        [Test]
        public void DeserializeOptional() {
            const int val      = 5;
            var       json     = $"[{val}]";
            var       fromJson = JsonConvert.DeserializeObject<Optional<int>>(json);
            Console.WriteLine($"fromJson: {fromJson}");
            Asserter.Against(fromJson)
                    .And(it => it.Value, Is.EqualTo(val))
                    .And(Is.EqualTo(new Optional<int>(val)))
                    .Invoke();
        }

        #endregion

        #region Flatten

        [Test]
        public void Flatten_2() {
            var inner = Optional.Of(5);
            var outer = Optional.Of(inner);
            var flat  = outer.Flatten();

            Asserter.Against(flat)
                    .And(Is.TypeOf<Optional<int>>())
                    .And(it => it.HasValue, Is.EqualTo(true))
                    .And(it => it.Value,    Is.EqualTo(inner.Value))
                    .Invoke();
        }

        [Test]
        public void Flatten_2_Empty() {
            var inner = Optional.Empty<int>();
            var outer = Optional.Of(inner);
            var flat  = outer.Flatten();

            Asserter.Against(flat)
                    .And(Is.TypeOf<Optional<int>>())
                    .And(it => it.HasValue, Is.EqualTo(false))
                    .And(it => it.Value,    Throws.InvalidOperationException)
                    .Invoke();
        }

        [Test]
        public void Flatten_3() {
            var inner  = Optional.Of(5);
            var middle = Optional.Of(inner);
            var outer  = Optional.Of(middle);
            var flat   = outer.Flatten();

            Asserter.Against(flat)
                    .And(Is.TypeOf<Optional<int>>())
                    .And(it => it.HasValue, Is.EqualTo(true))
                    .And(it => it.Value,    Is.EqualTo(5))
                    .Invoke();
        }

        [Test]
        public void Flatten_3_Empty() {
            var inner  = Optional.Empty<int>();
            var middle = Optional.Of(inner);
            var outer  = Optional.Of(middle);
            var flat   = outer.Flatten();

            Asserter.Against(flat)
                    .And(Is.TypeOf<Optional<int>>())
                    .And(it => it.HasValue, Is.EqualTo(false))
                    .And(it => it.Value,    Throws.InvalidOperationException)
                    .Invoke();
        }

        [Test]
        public void Flatten_4() {
            var t1   = Optional.Of(5);
            var t2   = Optional.Of(t1);
            var t3   = Optional.Of(t2);
            var t4   = Optional.Of(t3);
            var flat = t4.Flatten();

            Asserter.Against(flat)
                    .And(Is.TypeOf<Optional<int>>())
                    .And(it => it.HasValue, Is.EqualTo(true))
                    .And(it => it.Value,    Is.EqualTo(5))
                    .Invoke();
        }

        [Test]
        public void Flatten_4_Empty() {
            var t1   = Optional.Empty<int>();
            var t2   = Optional.Of(t1);
            var t3   = Optional.Of(t2);
            var t4   = Optional.Of(t3);
            var flat = t4.Flatten();

            Asserter.Against(flat)
                    .And(Is.TypeOf<Optional<int>>())
                    .And(it => it.HasValue, Is.EqualTo(false))
                    .And(it => it.Value,    Throws.InvalidOperationException)
                    .Invoke();
        }

        [Test]
        public void Flatten_Mixed() {
            var inner = Optional.Empty<Optional<int>>();
            var outer = Optional.Of(inner);
            var flat  = outer.Flatten();

            Asserter.Against(flat)
                    .And(Is.TypeOf<Optional<int>>())
                    .And(it => it.HasValue,    Is.False)
                    .And(it => it.Value,       Throws.InvalidOperationException)
                    .And(() => outer.HasValue, Is.True)
                    .Invoke();
        }

        #endregion
    }
}