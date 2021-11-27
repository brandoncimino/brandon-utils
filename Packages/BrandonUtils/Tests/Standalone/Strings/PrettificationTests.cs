using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

using BrandonUtils.Standalone;
using BrandonUtils.Standalone.Chronic;
using BrandonUtils.Standalone.Collections;
using BrandonUtils.Standalone.Enums;
using BrandonUtils.Standalone.Reflection;
using BrandonUtils.Standalone.Strings;
using BrandonUtils.Standalone.Strings.Json;
using BrandonUtils.Standalone.Strings.Prettifiers;
using BrandonUtils.Testing;
using BrandonUtils.Tests.Standalone.Reflection;

using Newtonsoft.Json;

using NUnit.Framework;

using Is = NUnit.Framework.Is;
using List = BrandonUtils.Standalone.Collections.List;

namespace BrandonUtils.Tests.Standalone.Strings {
    public class PrettificationTests {
        [SetUp]
        public void SetDefaultPrettificationSettings() {
            Prettification.DefaultPrettificationSettings.VerboseLogging = true;
        }

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
        [TestCase(typeof(int),    "int",    "int",    "int")]
        [TestCase(typeof(object), "object", "object", "object")]
        [TestCase(typeof(float),  "float",  "float",  "float")]
        [TestCase(typeof(string), "string", "string", "string")]
        [TestCase(
            typeof(KeyValuePair<DayOfWeek, string>),
            "KeyValuePair<DayOfWeek, string>",
            "KeyValuePair<,>",
            "KeyValuePair<>"
        )]
        [TestCase(
            typeof(List<KeyValuePair<Collection<short>, uint>>),
            "List<KeyValuePair<Collection<short>, uint>>",
            "List<>",
            "List<>"
        )]
        [TestCase(typeof(DayOfWeek), "DayOfWeek", "DayOfWeek", "DayOfWeek")]
        public void PrettifyType(Type actualType, string expected_full, string expected_short, string expected_none) {
            var settings_full = new PrettificationSettings() {
                TypeLabelStyle = { Value = TypeNameStyle.Full }
            };
            var settings_short = new PrettificationSettings() {
                TypeLabelStyle = { Value = TypeNameStyle.Short }
            };
            var settings_none = new PrettificationSettings() {
                TypeLabelStyle = { Value = TypeNameStyle.None }
            };
            Console.WriteLine(
                new[] {
                    $"ToString: {actualType}",
                    $"Name:     {actualType.Name}",
                    $"FullName: {actualType.FullName}",
                    $"Prettify: {actualType.Prettify()}",
                    $"P. type:  {InnerPretty.PrettifyType(actualType, default)}",
                    $"Full:     {actualType.PrettifyType(settings_full)}",
                    $"Short:    {actualType.PrettifyType(settings_short)}",
                    $"None:     {actualType.PrettifyType(settings_none)}",
                    $"DeclaringType:    {actualType.DeclaringType}",
                    $"NameOrKeyword:    {actualType.NameOrKeyword()}"
                }.JoinLines()
            );
            Asserter.Against(actualType)
                    .And(it => it.PrettifyType(settings_full),  Is.EqualTo(expected_full))
                    .And(it => it.PrettifyType(settings_none),  Is.EqualTo(expected_none))
                    .And(it => it.PrettifyType(settings_short), Is.EqualTo(expected_short))
                    .Invoke();
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
string DayOfWeek
------ ---------
one    Monday   
two    Tuesday  
three  Wednesday";

            Assert.That(dic.Prettify().Trim(), Is.EqualTo(expectedString.Trim()));
        }

        [Test]
        public void PrettifyDictionaryInferredType() {
            var dic = new Dictionary<object, object>() {
                [1]     = new TrainCar(),
                ["two"] = new Duckmobile()
            };

            var actual = dic.Prettify();
            var expected = @"
IComparable IVehicle                                           
----------- ---------------------------------------------------
1           BrandonUtils.Tests.Standalone.Reflection.TrainCar  
two         BrandonUtils.Tests.Standalone.Reflection.Duckmobile";
            Console.WriteLine(actual);

            Assert.That(actual.Trim(), Is.EqualTo(expected.Trim()));
        }

        private static Expectation[] Tuple2Expectations = new[] {
            new Expectation() {
                Original = ("one", 1),
                Expected = "(one, 1)"
            },
            new Expectation() {
                Original = (new List<double>() {
                                1,
                                99,
                                double.PositiveInfinity
                            }, ("yolo", 80085)),
                Expected = $"(List<double>[1, 99, {double.PositiveInfinity}], (yolo, 80085))"
            },
            new Expectation() {
                Original = (double.PositiveInfinity, "beyond"),
                Expected = $"({double.PositiveInfinity}, beyond)"
            }
        };

        [Test]
        public void PrettifyTuple2_One() {
            var tuple2 = (List.Of(1, double.PositiveInfinity, double.NegativeInfinity), "yolo");

            Console.WriteLine(tuple2);
            Console.WriteLine(tuple2.Prettify());

            PrettifyTuple2(Tuple2Expectations[1]);
        }

        [Test]
        public void PrettifyTuple2([ValueSource(nameof(Tuple2Expectations))] Expectation expectation) {
            Console.WriteLine($"tuple: {expectation.Original}");
            Console.WriteLine($"tuptype: {expectation.Original.GetType()}");
            var actualString = expectation.Original.Prettify();
            Console.WriteLine($"actual:\n{actualString}");
            Console.WriteLine($"expected:\n{expectation.Expected}");
            5.IsPositive();
            Assert.That(actualString, Is.EqualTo(expectation.Expected.Trim()));
        }

        [Test]
        public void PrettifyKeyedList() {
            var prettifier = new Prettifier<KeyedList<object, object>>(InnerPretty.PrettifyKeyedList);

            var keyedList = new KeyedList<int, (int, string)>(it => it.Item1) { (1, "one"), (2, "two"), (99, "ninety-nine") };

            var expectedString = $@"
int (int, string)    
--- -----------------
1   (1, one)           
2   (2, two)         
99  (99, ninety-nine)";
            var actualPrettifiedString = prettifier.Prettify(keyedList);
            Console.WriteLine($"{nameof(actualPrettifiedString)}:\n{actualPrettifiedString}");
            Console.WriteLine($"{nameof(expectedString)}:\n{expectedString}");
            Assert.That(actualPrettifiedString.SplitLines(StringSplitOptions.RemoveEmptyEntries).TrimLines(), Is.EqualTo(expectedString.SplitLines(StringSplitOptions.RemoveEmptyEntries).TrimLines()));
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

            var prettifier = Prettification.FindPrettifier(keyedList.GetType(), default);

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
            var prettifier = Prettification.FindPrettifier(actualType, default);
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
                new Expectation() {
                    Original = ls,
                    Expected = "[1, 2, 3]",
                    Settings = new PrettificationSettings() {
                        TypeLabelStyle = { Value = TypeNameStyle.None }
                    }
                },
                new Expectation() {
                    Original = ls,
                    Expected = "List<int>[1, 2, 3]",
                },
                new Expectation() {
                    Original = ls,
                    Expected = "List<>[1, 2, 3]",
                    Settings = new PrettificationSettings() {
                        TypeLabelStyle = { Value = TypeNameStyle.Short }
                    }
                },
                new Expectation() {
                    Original = ls,
                    Settings = new PrettificationSettings() {
                        TypeLabelStyle = { Value = TypeNameStyle.Full }
                    },
                    Expected = "List<int>[1, 2, 3]"
                },
                new Expectation() {
                    Original = ls,
                    Settings = new PrettificationSettings() {
                        PreferredLineStyle = { Value = LineStyle.Multi },
                        TypeLabelStyle     = { Value = TypeNameStyle.Full }
                    },
                    Expected = @"
List<int>[
  1
  2
  3
]"
                },
                new Expectation() {
                    Original = new List<double>() {
                        1,
                        99,
                        double.PositiveInfinity
                    },
                    Expected = "List<double>[1, 99, ∞]"
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

        [Test]
        [TestCase(new[] { DayOfWeek.Monday, DayOfWeek.Friday }, "EnumSet<DayOfWeek>[Monday, Friday]")]
        public void PrettifyEnumSet(DayOfWeek[] set, string expectedString) {
            var enumSet = new EnumSet<DayOfWeek>(set);
            var pretty  = enumSet.Prettify();
            Console.WriteLine(pretty);
            Assert.That(pretty, Is.EqualTo(expectedString));
        }

        [Test]
        public void TypeLabelStyle([Values] TypeNameStyle style) {
            var settings = new PrettificationSettings() {
                TypeLabelStyle = { Value = style }
            };
            var type = typeof(Dictionary<int, List<string>>);
            Console.WriteLine(
                new Dictionary<object, object>() {
                    [nameof(type)]      = type,
                    [nameof(type.Name)] = type.Name,
                    ["pretty"]          = type.PrettifyType(settings),
                    ["style"]           = style
                }.Prettify(settings)
            );
        }

        [Test]
        public void PerformanceTest_Dictionary() {
            var settings = new PrettificationSettings();

            var ob = new Dictionary<DayOfWeek, string>() {
                [DayOfWeek.Monday]    = "Monntag",
                [DayOfWeek.Tuesday]   = "Zwei...tag?",
                [DayOfWeek.Wednesday] = "Mittwoch",
                [DayOfWeek.Thursday]  = "Donnerstag",
                [DayOfWeek.Friday]    = "Freitag",
                [DayOfWeek.Saturday]  = "Samstag",
                [DayOfWeek.Sunday]    = "Sonntag"
            };

            Assert.That(ob.GetType(), Is.EqualTo(typeof(Dictionary<DayOfWeek, string>)));

            void ViaExtension(object obj) {
                _ = obj.Prettify(settings);
            }

            // ReSharper disable once SuggestBaseTypeForParameter
            void ViaSpecific(Dictionary<DayOfWeek, string> obj) {
                _ = InnerPretty.PrettifyDictionary<DayOfWeek, string>(obj);
            }

            var comparison = MethodTimer.CompareExecutions(
                ob,
                ViaExtension,
                ViaSpecific,
                1000
            );

            Console.WriteLine(comparison);
        }

        [Test]
        [TestCase(typeof(IDictionary<(int?, List<DayOfWeek>), Stopwatch>))]
        public void PerformanceTest_Type(Type type) {
            const int iterations = 2000;

            var settings = new PrettificationSettings();

            var comparison = MethodTimer.CompareExecutions(
                (type, settings),
                Prettification.Prettify,
                InnerPretty.PrettifyType,
                iterations
            );

            Console.WriteLine(comparison);
            Assert.That(comparison.Faster, Is.EqualTo(AggregateExecutionComparison.Which.Second));
        }

        class Parent {
            public string Nickname;
        }

        class Child : Parent {
            public bool IsBehaved;
        }

        [Test]
        public static void PerformanceTest_ExactVsDerivedType() {
            var settings = new PrettificationSettings();

            var exactPrettifier = new Prettifier<Parent>((parent, prettySettings) => $"Parent (actually {parent.GetType().Prettify(prettySettings)}): {parent.Nickname}");
            Prettification.RegisterPrettifier(exactPrettifier);

            var exactType   = new Parent() { Nickname = "Parent_1" };
            var derivedType = new Child() { Nickname  = "Child_1", IsBehaved = false };

            Console.WriteLine(
                new Dictionary<object, object>() {
                    [nameof(exactType)]   = exactType.GetType().Prettify(settings),
                    [nameof(derivedType)] = derivedType.GetType().Prettify(settings)
                }
            );

            Assert.That(derivedType.GetType(), Is.EqualTo(typeof(Child)));
            Assert.That(exactType.GetType(),   Is.EqualTo(typeof(Parent)));

            const int iterations = 2000;

            var comparison = MethodTimer.CompareExecutions(
                (nameof(exactType), () => _ = exactType.Prettify(settings)),
                (nameof(derivedType), () => _ = derivedType.Prettify(settings)),
                iterations
            );

            Assert.That(comparison.Faster, Is.EqualTo(AggregateExecutionComparison.Which.First));
        }

        [Test]
        public void CloneSettings() {
            var original = new PrettificationSettings() {
                HeaderStyle          = { Value = HeaderStyle.None },
                TableHeaderSeparator = { Value = "💄" },
                NullPlaceholder      = { Value = "⛑" }
            };

            Console.WriteLine($"ORIGINAL: {original}");

            Console.WriteLine($"header style: {original.HeaderStyle}");
            Console.WriteLine($"\t-> pretty: {original.HeaderStyle.Prettify(new PrettificationSettings())}");
            Console.WriteLine($"separator: {original.TableHeaderSeparator}");

            Console.WriteLine($"JSON: {JsonConvert.SerializeObject(original)}");

            var copy = original.JsonClone();

            Console.WriteLine($"COPY: {copy}");

            Asserter.Against(copy)
                    .And(Has.Property(nameof(copy.HeaderStyle)).EqualTo(original.HeaderStyle))
                    .And(Has.Property(nameof(copy.TableHeaderSeparator)).EqualTo(original.TableHeaderSeparator))
                    .And(Has.Property(nameof(copy.NullPlaceholder)).EqualTo(original.NullPlaceholder))
                    .And(Is.Not.SameAs(original))
                    .Invoke();
        }
    }
}