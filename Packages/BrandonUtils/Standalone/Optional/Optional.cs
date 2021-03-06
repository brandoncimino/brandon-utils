﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

        /// <summary>
        /// Tests the underlying values of <see cref="a"/> and <see cref="b"/> for equality.
        /// <ul>
        /// <li>If <b>both</b> <see cref="a"/> and <see cref="b"/> are <b>empty</b>, then they are considered <b>equal</b>.</li>
        /// <li>If <b>both</b> <see cref="a"/> and <see cref="b"/> are <c>null</c>, then they are considered <b>equal</b>.</li>
        /// <li>A <c>null</c> <see cref="IOptional{T}"/> is <b><i>NOT</i></b></li> considered equal to an <see cref="IOptional{T}"/> with a <c>null</c> <see cref="IOptional{T}.Value"/>!
        /// </ul>
        /// </summary>
        /// <remarks>
        /// I made this so that I had the same logic across all of the different <see cref="IEquatable{T}"/> and <c>==</c>
        /// operator comparisons in <see cref="Optional{T}"/>, <see cref="Failable{TValue,TExcuse}"/>, etc.
        /// </remarks>
        /// <param name="a">aka "x", aka "left-hand side"</param>
        /// <param name="b">aka "y", aka "right-hand side"</param>
        /// <typeparam name="T">the actual type of the underlying <see cref="IOptional{T}.Value"/>s</typeparam>
        /// <returns>the equality of the underlying <see cref="IOptional{T}.Value"/>s of <see cref="a"/> and <see cref="b"/></returns>
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

        public static string ToString<T>(IOptional<T> optional) {
            return $"{optional.GetType().Name}<{typeof(T).Name}>[{(optional.HasValue ? (optional.Value == null ? "null" : optional.Value + "") : "")}]";
        }

        public static TOut IfOrElse<TIn, TOut>(this TIn? nullable, Func<TIn, TOut> ifPresent, Func<TOut> orElse) where TIn : struct {
            return nullable.HasValue ? ifPresent.Invoke(nullable.Value) : orElse.Invoke();
        }

        /**
         * <inheritdoc cref="IfOrElse{TIn}(IOptional{TIn},System.Action{TIn},System.Action)"/>
         */
        public static void IfOrElse<TIn>(this TIn? nullable, Action<TIn> ifPresent, Action orElse) where TIn : struct {
            if (nullable.HasValue) {
                ifPresent.Invoke(nullable.Value);
            }
            else {
                orElse.Invoke();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks>
        /// Mimic's Java's
        /// </remarks>
        /// <param name="optional"></param>
        /// <param name="ifPresent"></param>
        /// <param name="orElse"></param>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <returns></returns>
        public static TOut IfOrElse<TIn, TOut>(this IOptional<TIn> optional, Func<TIn, TOut> ifPresent, Func<TOut> orElse) {
            return optional.HasValue ? ifPresent.Invoke(optional.Value) : orElse.Invoke();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks>
        /// Mimics Java's <a href="https://docs.oracle.com/javase/9/docs/api/java/util/Optional.html#ifPresentOrElse-java.util.function.Consumer-java.lang.Runnable-">ifPresentOrElse()</a>
        /// </remarks>
        /// <param name="optional"></param>
        /// <param name="ifPresent"></param>
        /// <param name="orElse"></param>
        /// <typeparam name="TIn"></typeparam>
        public static void IfOrElse<TIn>(this IOptional<TIn> optional, Action<TIn> ifPresent, Action orElse) {
            if (optional.HasValue) {
                ifPresent.Invoke(optional.Value);
            }
            else {
                orElse.Invoke();
            }
        }

        /// <summary>
        /// If this <see cref="IOptional{T}.HasValue"/>, returns <see cref="IOptional{T}.Value"/>.
        ///
        /// Throws an exception if the <see cref="IOptional{T}"/> is empty.
        /// </summary>
        /// <param name="optional">an <see cref="IOptional{T}"/></param>
        /// <param name="exceptionProvider">the <see cref="Func{TResult}"/> that generates the exception. Defaults to <see cref="NoCanHasValue{T}"/></param>
        /// <typeparam name="T">the underlying type of the <see cref="IOptional{T}"/></typeparam>
        /// <returns>the <see cref="IOptional{T}.Value"/></returns>
        /// <exception cref="Exception">if the <see cref="IOptional{T}"/> is empty</exception>
        public static T OrElseThrow<T>(this IOptional<T> optional, Func<Exception> exceptionProvider = default) {
            return optional.HasValue ? optional.Value : throw (exceptionProvider?.Invoke() ?? NoCanHasValue(optional));
        }

        /// <summary>
        /// If this <see cref="Nullable{T}.HasValue"/>, returns <see cref="Nullable{T}.Value"/>.
        ///
        /// Throws and exception if the <see cref="Nullable{T}"/> is empty.
        /// </summary>
        /// <param name="nullable">a <see cref="Nullable{T}"/></param>
        /// <param name="exceptionProvider"><inheritdoc cref="OrElseThrow{T}(BrandonUtils.Standalone.Optional.IOptional{T},System.Func{System.Exception})"/></param>
        /// <typeparam name="T">the underlying type of the <see cref="Nullable{T}"/></typeparam>
        /// <returns></returns>
        /// <exception cref="Exception">if the <see cref="Nullable{T}"/> is empty</exception>
        public static T OrElseThrow<T>(this T? nullable, Func<Exception> exceptionProvider = default) where T : struct {
            return nullable ?? throw (exceptionProvider?.Invoke() ?? NoCanHasValue<T?>());
        }

        private static ArgumentNullException NoCanHasValue<T>(T emptyThing = default) {
            return new ArgumentNullException($"The {typeof(T).Name} was empty!");
        }

        /// <returns>negation of <see cref="IOptional{T}.HasValue"/></returns>
        public static bool IsEmpty<T>(this IOptional<T> optional) {
            return !optional.HasValue;
        }

        /// <returns>negation of <see cref="Nullable{T}.HasValue"/></returns>
        public static bool IsEmpty<T>(this T? nullable) where T : struct {
            return !nullable.HasValue;
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