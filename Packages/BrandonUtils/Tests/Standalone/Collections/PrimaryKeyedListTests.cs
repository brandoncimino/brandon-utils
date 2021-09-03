using System;
using System.Collections.Generic;
using System.Linq;

using BrandonUtils.Standalone.Collections;

using Newtonsoft.Json;

using NUnit.Framework;

namespace BrandonUtils.Tests.Standalone.Collections {
    public class PrimaryKeyedListTests {
        private class NoInterface : IEquatable<NoInterface> {
            public DayOfWeek DayOfWeek;
            public string    Info;

            public NoInterface(DayOfWeek dayOfWeek) {
                this.DayOfWeek = dayOfWeek;
                this.Info      = $"DEFAULT {nameof(Info)} for {dayOfWeek}";
            }

            public bool Equals(NoInterface other) {
                return other != null && other.DayOfWeek == DayOfWeek && other.Info == Info;
            }
        }

        private class HasInterface : NoInterface, IPrimaryKeyed<DayOfWeek> {
            public DayOfWeek PrimaryKey => DayOfWeek;

            public HasInterface(DayOfWeek dayOfWeek) : base(dayOfWeek) { }

            public override string ToString() {
                return JsonConvert.SerializeObject(this);
            }
        }

        private class AlsoHasInterface : NoInterface, IPrimaryKeyed<DayOfWeek> {
            public DayOfWeek PrimaryKey => DayOfWeek;

            public AlsoHasInterface(DayOfWeek dayOfWeek) : base(dayOfWeek) { }
        }

        private static KeyedList<DayOfWeek, HasInterface> _keyedList = new PrimaryKeyedList<DayOfWeek, HasInterface> {
            new HasInterface(DayOfWeek.Monday),
            new HasInterface(DayOfWeek.Tuesday),
            new HasInterface(DayOfWeek.Wednesday),
            new HasInterface(DayOfWeek.Thursday),
            new HasInterface(DayOfWeek.Friday),
            new HasInterface(DayOfWeek.Saturday),
            new HasInterface(DayOfWeek.Sunday)
        };

        private class StringKey : IPrimaryKeyed<string>, IEquatable<StringKey> {
            public string Name;
            public int    Number;
            public string PrimaryKey => Name;

            public StringKey(string name, int number) {
                Name   = name;
                Number = number;
            }

            public bool Equals(StringKey other) {
                return other != null && other.Name == Name && other.Number == Number;
            }
        }

        private static KeyedList<string, StringKey> StringKeyedList = new PrimaryKeyedList<string, StringKey>() {
            new StringKey("one",   1),
            new StringKey("two",   2),
            new StringKey("three", 3)
        };

        [Test]
        public void KeyedListSerializes() {
            string json = JsonConvert.SerializeObject(_keyedList, Formatting.Indented);

            Console.WriteLine(json);

            var kl = JsonConvert.DeserializeObject<KeyedList<DayOfWeek, HasInterface>>(json);

            Assert.That(_keyedList, Is.EqualTo(kl));
        }

        [Test]
        public void StringKeySerializes() {
            string json = JsonConvert.SerializeObject(StringKeyedList, Formatting.Indented);

            Console.WriteLine(json);

            var kl = JsonConvert.DeserializeObject<KeyedList<string, StringKey>>(json);

            Assert.That(kl, Is.EqualTo(StringKeyedList));
        }

        [Test]
        public void KeyedListPassAsList() {
            GimmeAList(_keyedList);
        }

        private void GimmeAList<T>(IList<T> list) {
            Assert.Pass();
        }

        [Test]
        public void GetViaKey() {
            Assert.That(_keyedList[DayOfWeek.Monday], Is.EqualTo(new HasInterface(DayOfWeek.Monday)));
        }

        [Test]
        public void Cloneable() {
            var copy = _keyedList.Copy();

            Assert.That(copy, Is.EqualTo(_keyedList));
        }

        [Test]
        public void UpdateViaKey() {
            var copy = _keyedList.Copy();

            const string updatedInfo = "UPDATED";
            copy[DayOfWeek.Monday].Info = updatedInfo;

            for (int i = 0; i > _keyedList.Count; i++) {
                Assert.That(copy.ElementAt(i).Info, Is.EqualTo(_keyedList.ElementAt(i).PrimaryKey == DayOfWeek.Monday ? updatedInfo : _keyedList.ElementAt(i).Info));
            }
        }

        [Test]
        public void AddViaKey() {
            var kl = new PrimaryKeyedList<DayOfWeek, HasInterface>() {
                new HasInterface(DayOfWeek.Monday)
            };

            int initialCount = kl.Count;

            kl[DayOfWeek.Tuesday] = new HasInterface(DayOfWeek.Tuesday);

            Assert.That(kl, Has.Property(nameof(kl.Count)).EqualTo(initialCount + 1));
            Assert.That(kl, Contains.Item(new HasInterface(DayOfWeek.Tuesday)));
        }

        [Test]
        public void ErrorAddingDuplicateKey() {
            var copy = _keyedList.Copy();
            Assert.Throws<ArgumentException>(() => copy.Add(new HasInterface(DayOfWeek.Monday)));
        }

        [Test]
        public void ErrorCreatingFromDuplicateList() {
            var ls = new List<HasInterface> {
                new HasInterface(DayOfWeek.Monday),
                new HasInterface(DayOfWeek.Tuesday),
                new HasInterface(DayOfWeek.Tuesday),
                new HasInterface(DayOfWeek.Tuesday)
            };
            Assert.Throws<ArgumentException>(() => new PrimaryKeyedList<DayOfWeek, HasInterface>(ls));
        }

        [Test]
        public void CanCreateFromValidList() {
            var ls = new List<HasInterface> {
                new HasInterface(DayOfWeek.Monday),
                new HasInterface(DayOfWeek.Tuesday),
                new HasInterface(DayOfWeek.Wednesday)
            };

            var kl = new PrimaryKeyedList<DayOfWeek, HasInterface> {
                new HasInterface(DayOfWeek.Monday),
                new HasInterface(DayOfWeek.Tuesday),
                new HasInterface(DayOfWeek.Wednesday)
            };

            Assert.That(new PrimaryKeyedList<DayOfWeek, HasInterface>(ls), Is.EqualTo(kl));
        }

        [Test]
        public void ErrorOnMissingKey() {
            var kl = new PrimaryKeyedList<DayOfWeek, HasInterface>() {
                new HasInterface(DayOfWeek.Tuesday)
            };

            Assert.Throws<KeyNotFoundException>(() => kl[DayOfWeek.Monday].Info = "yolo");
        }

        [Test]
        public void ErrorOnRemovingMissingKey() {
            var kl = new PrimaryKeyedList<DayOfWeek, HasInterface>() {
                new HasInterface(DayOfWeek.Tuesday)
            };

            Assert.False(kl.Remove(DayOfWeek.Monday));
        }

        [Test]
        public void TestRemove() {
            var kl = new PrimaryKeyedList<DayOfWeek, HasInterface>() {
                new HasInterface(DayOfWeek.Monday),
                new HasInterface(DayOfWeek.Tuesday)
            };

            var initialLength = kl.Count;

            Assert.True(kl.Remove(DayOfWeek.Monday));
            Assert.That(kl, Contains.Item(new HasInterface(DayOfWeek.Tuesday)));
            Assert.That(kl, Does.Not.Contains(new HasInterface(DayOfWeek.Monday)));
            Assert.That(kl, Has.Property(nameof(kl.Count)).EqualTo(initialLength - 1));
        }

        [Test]
        public void TestJsonPopulateWithSameData() {
            var kl = new PrimaryKeyedList<DayOfWeek, HasInterface> {
                new HasInterface(DayOfWeek.Monday),
                new HasInterface(DayOfWeek.Tuesday)
            };

            var json = JsonConvert.SerializeObject(kl);
            JsonConvert.PopulateObject(json, kl);
        }

        [Test]
        public void TestJsonPopulateEmpty() {
            var kl = new PrimaryKeyedList<DayOfWeek, HasInterface> {
                new HasInterface(DayOfWeek.Monday),
                new HasInterface(DayOfWeek.Tuesday)
            };

            var json = JsonConvert.SerializeObject(kl);

            var kl2 = new PrimaryKeyedList<DayOfWeek, HasInterface>();

            Assert.That(kl2, Is.Not.EqualTo(kl));

            JsonConvert.PopulateObject(json, kl2);

            Assert.That(kl2, Is.EqualTo(kl));
        }
    }
}