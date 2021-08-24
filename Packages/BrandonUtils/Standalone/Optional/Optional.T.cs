using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Optional {
    /// <inheritdoc cref="IOptional{T}"/>
    /// <remarks>
    /// An <see cref="Optional{T}"/> can be considered a <see cref="IReadOnlyCollection{T}"/> with a max capacity of 1.
    /// This gives access to the full suite of <see cref="System.Linq"/> extension methods.
    /// </remarks>
    [PublicAPI]
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
            _items = new List<T> { value };
        }

        #region Implementations

        public IEnumerator<T> GetEnumerator() {
            return EnumerableImplementation.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return ((IEnumerable)EnumerableImplementation).GetEnumerator();
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
            // return $"{GetType().Name}<{this.ItemType().Name}>[{(HasValue ? (Value == null ? "null" : Value+"") : "")}]";
            return Optional.ToString(this);
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