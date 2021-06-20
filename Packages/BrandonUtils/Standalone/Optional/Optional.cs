using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using BrandonUtils.Standalone.Collections;

namespace BrandonUtils.Standalone.Optional {
    /// <summary>
    /// Utility and extension methods for <see cref="Optional{T}"/>.
    /// </summary>
    public static class Optional {
        /// <summary>
        /// Creates an <see cref="Optional{T}"/> without ugly type parameters.
        /// </summary>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Optional<T> Of<T>(T value) {
            return new Optional<T>(value);
        }

        /// <summary>
        /// "Collects" an <see cref="IEnumerable{T}"/> as an <see cref="Optional{T}"/>.
        ///
        /// This works similarly to <see cref="Enumerable.Single{TSource}(System.Collections.Generic.IEnumerable{TSource})"/>, except:
        ///
        /// <ul>
        /// <li>If <see cref="source"/> is empty ⇒ return an empty <see cref="Optional{T}"/>.</li>
        /// <li>If <see cref="source"/> is <c>null</c> ⇒ return an empty <see cref="Optional{T}"/>.</li>
        /// <li>If <see cref="source"/> has more than 1 entry ⇒ throw an <see cref="InvalidOperationException"/>.</li>
        /// </ul>
        ///
        /// </summary>
        /// <param name="source">an <see cref="IEnumerable{T}"/> with 0 or 1 elements</param>
        /// <typeparam name="T">the type of <see cref="source"/></typeparam>
        /// <returns>an <see cref="Optional{T}"/> containing the 0 or 1 elements of <see cref="source"/></returns>
        /// <exception cref="InvalidOperationException">If <see cref="source"/> has more than 1 element</exception>
        /// <remarks>
        /// Corresponds to Guava's <a href="https://guava.dev/releases/21.0/api/docs/com/google/common/collect/MoreCollectors.html#toOptional--">MoreCollectors.toOptional()</a>.
        /// </remarks>
        public static Optional<T> ToOptional<T>(this IEnumerable<T> source) {
            if (source == null) {
                return default;
            }

            var ls = source.ToList();
            return ls.Any() ? Of(ls.Single()) : default;
        }

        /// <summary>
        /// Attempts to <see cref="Func{TResult}.Invoke"/> <see cref="functionThatMightFail"/>, returning a <see cref="Failable{TValue,TException}"/>
        /// that contains either the successful result or the reason for failure.
        /// </summary>
        /// <param name="functionThatMightFail"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Failable<T> Try<T>(this Func<T> functionThatMightFail) {
            return new Failable<T>(functionThatMightFail);
        }

        /// <summary>
        /// Returns <see cref="IOptional{T}.Value"/> if it is present; otherwise, returns <see cref="fallback"/>.
        /// </summary>
        /// <remarks>
        /// Corresponds to:
        /// <ul>
        /// <li><see cref="Nullable{T}"/><see cref="Nullable{T}.GetValueOrDefault()"/></li>
        /// <li>Java's <a href="https://docs.oracle.com/javase/8/docs/api/java/util/Optional.html#orElse-T-">Optional.orElse()</a></li>
        /// </ul>
        /// </remarks>
        /// <param name="optional"></param>
        /// <param name="fallback"></param>
        /// <returns></returns>
        public static T GetValueOrDefault<T>(this IOptional<T> optional, T fallback) {
            return optional.HasValue ? optional.Value : fallback;
        }

        /// <summary>
        /// Returns <see cref="IOptional{T}.Value"/> if it is present; otherwise, <see cref="Func{TResult}.Invoke"/>s <see cref="fallbackSupplier"/>.
        /// </summary>
        /// <remarks>
        /// Corresponds to Java's <a href="https://docs.oracle.com/javase/8/docs/api/java/util/Optional.html#orElseGet-java.util.function.Supplier-">Optional.orElseGet()</a>.
        /// </remarks>
        /// <param name="optional">this <see cref="IOptional{T}"/></param>
        /// <param name="fallbackSupplier">a <see cref="Func{TResult}"/> that produces a <see cref="T"/> if <see cref="IOptional{T}.Value"/> isn't present</param>
        /// <returns><see cref="IOptional{T}.Value"/> if this <see cref="IOptional{T}.HasValue"/>; otherwise, <see cref="Func{TResult}.Invoke"/>s <see cref="fallbackSupplier"/></returns>
        public static T GetValueOrDefault<T>(this IOptional<T> optional, Func<T> fallbackSupplier) {
            return optional.HasValue ? optional.Value : fallbackSupplier.Invoke();
        }

        public static bool AreEqual<T>(IOptional<T> a, IOptional<T> b) {
            if (ReferenceEquals(a, b)) {
                return true;
            }

            if (ReferenceEquals(a, null) || ReferenceEquals(b, null)) {
                return false;
            }

            Console.WriteLine($"a: {a.HasValue}");

            if (a.HasValue == b.HasValue) {
                return !a.HasValue || Equals(a.Value, b.Value);
            }

            return false;
        }

        public static bool AreEqual<T>(IOptional<T> a, T b) {
            // We should be comparing the _value_ of `a` to `b`, which means a value has to exist
            if (ReferenceEquals(a, null)) {
                return false;
            }

            return a.HasValue && a.Value.Equals(b);
        }

        public static bool AreEqual<T>(T a, IOptional<T> b) {
            return AreEqual(b, a);
        }
    }

    /// <summary>
    /// A mockery of Java's <a href="https://docs.oracle.com/javase/8/docs/api/java/util/Optional.html">Optional</a> class.
    /// </summary>
    /// <remarks>
    /// An <see cref="Optional{T}"/> can be considered a <see cref="IReadOnlyCollection{T}"/> with a max capacity of 1.
    /// This gives access to the full suite of <see cref="System.Linq"/> extension methods.
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public readonly struct Optional<T> : IEquatable<T>, IEquatable<IOptional<T>>, IReadOnlyCollection<T>, IOptional<T> {
        public override bool Equals(object obj) {
            return obj switch {
                Optional<T> optional when Equals(optional) => true,
                T t when Equals(t)                         => true,
                _                                          => false
            };
        }

        public override int GetHashCode() {
            return (_items != null ? _items.GetHashCode() : 0);
        }

        private readonly List<T> _items;

        private IEnumerable<T> EnumerableImplementation => HasValue ? _items : Enumerable.Empty<T>();

        /// <summary>
        /// Whether or not a <see cref="Value"/> is present.
        /// </summary>
        /// <remarks>
        /// Corresponds to:
        /// <ul>
        /// <li><see cref="Nullable{T}"/>.<see cref="Nullable{T}.HasValue"/></li>
        /// <li>Java's <a href="https://docs.oracle.com/javase/8/docs/api/java/util/Optional.html#isPresent--">Optional.isPresent()</a></li>
        /// </ul>
        /// </remarks>
        public bool HasValue => _items != null;

        public T Value => HasValue ? _items[0] : throw new InvalidOperationException($"Unable to retrieve the {nameof(Value)} from the {GetType().Name} because it is empty!");

        public Optional(T value) {
            _items = new List<T> {value};
        }

        #region Implementations

        public IEnumerator<T> GetEnumerator() {
            return EnumerableImplementation.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return ((IEnumerable) EnumerableImplementation).GetEnumerator();
        }

        public bool Equals(T other) {
            return Optional.AreEqual(this, other);
        }

        public bool Equals(IOptional<T> other) {
            return Optional.AreEqual(this, other);
        }

        public static bool operator ==(Optional<T> a, IOptional<T> b) {
            return Optional.AreEqual(a, b);
        }

        public static bool operator !=(Optional<T> a, IOptional<T> b) {
            return !Optional.AreEqual(a, b);
        }

        public static bool operator ==(T a, Optional<T> b) {
            return Optional.AreEqual(a, b);
        }

        public static bool operator !=(T a, Optional<T> b) {
            return !Optional.AreEqual(a, b);
        }

        public static bool operator ==(Optional<T> a, T b) {
            return Optional.AreEqual(a, b);
        }

        public static bool operator !=(Optional<T> a, T b) {
            return !Optional.AreEqual(a, b);
        }

        public int Count => HasValue ? 1 : 0;

        public override string ToString() {
            return $"{nameof(Optional<T>)}<{this.ItemType().Name}>[{(HasValue ? Value + "" : "")}]";
        }

        #endregion

        public static implicit operator Optional<T>(T value) {
            return new Optional<T>(value);
        }

        /// <summary>
        /// Works the same as <see cref="Enumerable.Select{TSource,TResult}(System.Collections.Generic.IEnumerable{TSource},System.Func{TSource,TResult})">Enumerable.Select</see>,
        /// but returns a new <see cref="Optional{T}"/>.
        /// </summary>
        /// <remarks>
        /// Corresponds to:
        /// <ul>
        /// <li><see cref="Enumerable.Select{TSource,TResult}(System.Collections.Generic.IEnumerable{TSource},System.Func{TSource,TResult})"/></li>
        /// <li>Java's <a href="https://docs.oracle.com/javase/8/docs/api/java/util/Optional.html#map-java.util.function.Function-">Optional.map()</a></li>
        /// </ul>
        /// </remarks>
        /// <param name="selector"></param>
        /// <typeparam name="TNew"></typeparam>
        /// <returns></returns>
        public Optional<TNew> Select<TNew>(Func<T, TNew> selector) {
            return HasValue ? new Optional<TNew>(selector.Invoke(Value)) : new Optional<TNew>();
        }

        /// <summary>
        /// If <see cref="HasValue"/> and <see cref="Value"/> satisfies <paramref name="predicate"/>, return this <see cref="Optional{T}"/>.
        ///
        /// Otherwise, return an empty <see cref="Optional{T}"/>.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public Optional<T> Where(Func<T, bool> predicate) {
            if (HasValue && predicate.Invoke(Value)) {
                return this;
            }
            else {
                return default;
            }
        }
    }
}