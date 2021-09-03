using System;
using System.Collections.Generic;
using System.Linq;

using BrandonUtils.Standalone.Collections;
using BrandonUtils.Standalone.Enums;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Optional {
    /// <summary>
    /// Utility and extension methods for <see cref="Optional{T}"/>.
    /// </summary>
    [PublicAPI]
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
        /// Attempts to <see cref="Func{TResult}.Invoke"/> <see cref="functionThatMightFail"/>, returning a <see cref="FailableFunc{TValue,TExcuse}"/>
        /// that contains either the successful result or the <see cref="IFailableFunc{TValue,TExcuse}.Excuse"/> for failure.
        /// </summary>
        /// <param name="functionThatMightFail"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <example>
        /// TODO: Add an example, but I'm tired right now and when I started writing one instead made <see cref="DayOfWeekExtensions.IsSchoolNight"/>
        /// </example>
        public static FailableFunc<T> Try<T>(this Func<T> functionThatMightFail) {
            return new FailableFunc<T>(functionThatMightFail);
        }

        /// <summary>
        /// Attempts to <see cref="Action.Invoke"/> <see cref="actionThatMightFail"/>, returning a <see cref="Failable"/>
        /// that (might) contain the <see cref="IFailableFunc{TValue,TExcuse}.Excuse"/> for failure.
        /// </summary>
        /// <param name="actionThatMightFail"></param>
        /// <returns></returns>
        public static Failable Try(this Action actionThatMightFail) {
            return new Failable(actionThatMightFail);
        }

        #region GetValueOrDefault

        /// <summary>
        /// Returns <see cref="IOptional{T}.Value"/> if this <see cref="IOptional{T}.HasValue"/>; otherwise, returns <see cref="fallback"/>.
        /// </summary>
        /// <remarks>
        /// Corresponds to:
        /// <ul>
        /// <li><see cref="Nullable{T}"/>.<see cref="Nullable{T}.GetValueOrDefault()"/></li>
        /// <li>Java's <a href="https://docs.oracle.com/javase/8/docs/api/java/util/Optional.html#orElse-T-">Optional.orElse()</a></li>
        /// </ul>
        /// </remarks>
        /// <param name="optional">this <see cref="IOptional{T}"/></param>
        /// <param name="fallback">the return value if this <see cref="CollectionUtils.IsEmpty{T}"/></param>
        /// <returns><see cref="IOptional{T}.Value"/> if it this <see cref="IOptional{T}.HasValue"/>; otherwise, returns <see cref="fallback"/>.</returns>
        public static T GetValueOrDefault<T>([CanBeNull] [ItemCanBeNull] this IOptional<T> optional, [CanBeNull] T fallback) {
            return optional is { HasValue: true } ? optional.Value : fallback;
        }

        /// <summary>
        /// Returns <see cref="IOptional{T}.Value"/> if it is present; otherwise, <see cref="Func{TResult}.Invoke"/>s <see cref="fallbackSupplier"/>.
        /// </summary>
        /// <remarks>
        /// Corresponds to Java's <a href="https://docs.oracle.com/javase/8/docs/api/java/util/Optional.html#orElseGet-java.util.function.Supplier-">Optional.orElseGet()</a>,
        /// but with C#-style naming that matches <see cref="Nullable{T}"/>.<see cref="Nullable{T}.GetValueOrDefault()"/>.
        /// </remarks>
        /// <param name="optional">this <see cref="IOptional{T}"/></param>
        /// <param name="fallbackSupplier">a <see cref="Func{TResult}"/> that produces a <see cref="T"/> if <see cref="IOptional{T}.Value"/> isn't present</param>
        /// <returns><see cref="IOptional{T}.Value"/> if this <see cref="IOptional{T}.HasValue"/>; otherwise, <see cref="Func{TResult}.Invoke"/>s <see cref="fallbackSupplier"/></returns>
        public static T GetValueOrDefault<T>([CanBeNull] this IOptional<T> optional, [NotNull] Func<T> fallbackSupplier) {
            if (fallbackSupplier == null) {
                throw new ArgumentNullException(nameof(fallbackSupplier));
            }

            // woah...this some new-fangled way to say:
            // return optional != null && optional.HasValue ? optional.Value : fallbackSupplier.Invoke();
            return optional is { HasValue: true } ? optional.Value : fallbackSupplier.Invoke();
        }

        #endregion

        #region AreEqual

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
        /// operator comparisons in <see cref="Optional{T}"/>, <see cref="FailableFunc{TValue}"/>, etc.
        /// <p/>
        /// In fancy language, this method provides a default equality comparator for <see cref="IOptional{T}"/> implementations.
        /// </remarks>
        /// <param name="a">aka "x", aka "left-hand side"</param>
        /// <param name="b">aka "y", aka "right-hand side"</param>
        /// <typeparam name="T">the actual type of the underlying <see cref="IOptional{T}.Value"/>s</typeparam>
        /// <returns>the equality of the underlying <see cref="IOptional{T}.Value"/>s of <see cref="a"/> and <see cref="b"/></returns>
        /// <seealso cref="AreEqual{T}(BrandonUtils.Standalone.Optional.IOptional{T},T)"/>
        /// <seealso cref="AreEqual{T}(T,BrandonUtils.Standalone.Optional.IOptional{T})"/>
        public static bool AreEqual<T>([CanBeNull] IOptional<T> a, [CanBeNull] IOptional<T> b) {
            // return true if EITHER:
            // - a & b are the same object, OR
            // - a & b are both null
            if (ReferenceEquals(a, b)) {
                return true;
            }

            // since at this point we know that they can't _both_ be null, if _either_ of them is null, return false
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null)) {
                return false;
            }

            // if a & b are both EITHER:
            // - not empty, OR
            // - empty
            if (a.HasValue == b.HasValue) {
                // if a is empty, then b is empty, and two empties are considered to match, so we return TRUE
                // otherwise, we compare the actual values
                return !a.HasValue || Equals(a.Value, b.Value);
            }

            return false;
        }

        /// <summary>
        /// Compares the <see cref="IOptional{T}.Value"/> of an <see cref="IOptional{T}"/> to a vanilla <typeparamref name="T"/> value
        /// </summary>
        /// <remarks>
        /// <ul>
        /// <li>A null <see cref="IOptional{T}"/> should <b>not</b> be considered equal to a null <typeparamref name="T"/></li>
        /// <li>Anm
        /// </ul>
        /// </remarks>
        /// <param name="a">an <see cref="IOptional{T}"/></param>
        /// <param name="b">a vanilla <typeparamref name="T"/> value</param>
        /// <typeparam name="T">the underlying type being compared</typeparam>
        /// <returns>the equality of (<paramref name="a"/>.<see cref="IOptional{T}.Value"/>) and (<paramref name="b"/>)</returns>
        /// <seealso cref="AreEqual{T}(BrandonUtils.Standalone.Optional.IOptional{T},BrandonUtils.Standalone.Optional.IOptional{T})"/>
        /// <seealso cref="AreEqual{T}(T,BrandonUtils.Standalone.Optional.IOptional{T})"/>
        public static bool AreEqual<T>([CanBeNull] IOptional<T> a, [CanBeNull] T b) {
            // this method compares the _value_ of `a` to `b`, which means a value has to exist
            if (ReferenceEquals(a, null)) {
                return false;
            }

            return a.HasValue && Equals(a.Value, b);
        }

        /// <summary>
        /// Compares a vanilla <typeparamref name="T"/> value to the underlying <see cref="IOptional{T}.Value"/> of an <see cref="IOptional{T}"/>.
        /// </summary>
        /// <remarks>
        /// Behind the scenes, this flips <paramref name="a"/> and <paramref name="b"/> and sends them to <see cref="AreEqual{T}(BrandonUtils.Standalone.Optional.IOptional{T},T)"/>
        /// </remarks>
        /// <param name="a">a vanilla <typeparamref name="T"/> value</param>
        /// <param name="b">an <see cref="IOptional{T}"/></param>
        /// <typeparam name="T">the underlying type being compared</typeparam>
        /// <returns>the equality of (<paramref name="a"/>) and (<paramref name="b"/>.<see cref="IOptional{T}.Value"/>)</returns>
        /// <seealso cref="AreEqual{T}(BrandonUtils.Standalone.Optional.IOptional{T},BrandonUtils.Standalone.Optional.IOptional{T})"/>
        /// <seealso cref="AreEqual{T}(BrandonUtils.Standalone.Optional.IOptional{T},T)"/>
        public static bool AreEqual<T>([CanBeNull] T a, [CanBeNull] IOptional<T> b) {
            return AreEqual(b, a);
        }

        #endregion AreEqual

        /// <summary>
        /// Provides a "default" <see cref="object.ToString"/> method for <see cref="IOptional{T}"/> implementations to use.
        /// </summary>
        /// <param name="optional">an <see cref="IOptional{T}"/></param>
        /// <typeparam name="T">the underlying type of the <see cref="IOptional{T}"/></typeparam>
        /// <returns>a <see cref="object.ToString"/> representation of the given <see cref="IOptional{T}"/></returns>
        public static string ToString<T>([CanBeNull] IOptional<T> optional) {
            return optional == null
                       ? $"(IOptional<{typeof(T).Name}>)null"
                       : $"{optional.GetType().Name}<{typeof(T).Name}>[{(optional.HasValue ? (optional.Value == null ? "null" : optional.Value + "") : "")}]";
        }

        #region IfPresentOrElse

        /**
         * <inheritdoc cref="IfPresentOrElse{TIn,TOut}(IOptional{TIn},System.Func{TIn,TOut},System.Func{TOut})"/>
         */
        public static TOut IfPresentOrElse<TIn, TOut>(this TIn? nullable, Func<TIn, TOut> ifPresent, Func<TOut> orElse) where TIn : struct {
            return nullable.HasValue ? ifPresent.Invoke(nullable.Value) : orElse.Invoke();
        }

        /**
         * <inheritdoc cref="IfPresentOrElse{TIn,TOut}(IOptional{TIn},System.Func{TIn,TOut},System.Func{TOut})"/>
         */
        public static void IfPresentOrElse<TIn>(this TIn? nullable, Action<TIn> ifPresent, Action orElse) where TIn : struct {
            if (nullable.HasValue) {
                ifPresent.Invoke(nullable.Value);
            }
            else {
                orElse.Invoke();
            }
        }

        /// <summary>
        /// If this <see cref="IOptional{T}.HasValue"/>, return the result of <paramref name="ifPresent"/>.
        /// Otherwise, return the result of <paramref name="orElse"/>.
        /// </summary>
        /// <remarks>
        /// Doesn't mimic one of Java's <a href="https://docs.oracle.com/javase/9/docs/api/java/util/Optional.html">Optional</a> methods directly,
        /// but is equivalent to calling:
        /// <code><![CDATA[//JAVA
        /// Optional<Integer> optional;
        ///
        /// optional.map(it -> ifPresent(it))
        ///     .orElseGet(() -> orElse())
        /// ]]></code>
        /// I suppose it could be called "<c>IfPresentOrElseGet</c>", but that's silly
        /// </remarks>
        /// <param name="optional">this <see cref="IOptional{T}"/></param>
        /// <param name="ifPresent">the "mapping" <see cref="Func{TIn, TResult}"/> to run if this <see cref="IOptional{T}.HasValue"/></param>
        /// <param name="orElse">the <see cref="Func{TResult}"/> that generates the "default" / "fallback" value when this <see cref="IsEmpty{T}(BrandonUtils.Standalone.Optional.IOptional{T})"/></param>
        /// <typeparam name="TIn">the underlying type of the original <see cref="IOptional{T}"/></typeparam>
        /// <typeparam name="TOut">the resulting type</typeparam>
        /// <returns></returns>
        public static TOut IfPresentOrElse<TIn, TOut>(this IOptional<TIn> optional, Func<TIn, TOut> ifPresent, Func<TOut> orElse) {
            return optional.HasValue ? ifPresent.Invoke(optional.Value) : orElse.Invoke();
        }

        /// <summary>
        /// If this <see cref="IOptional{T}.HasValue"/>, executes <paramref name="ifPresent"/>.
        /// Otherwise, executes <paramref name="orElse"/>.
        /// </summary>
        /// <remarks>
        /// Mimics Java's <a href="https://docs.oracle.com/javase/9/docs/api/java/util/Optional.html#ifPresentOrElse-java.util.function.Consumer-java.lang.Runnable-">ifPresentOrElse()</a>
        /// </remarks>
        /// <param name="optional">this <see cref="IOptional{T}"/></param>
        /// <param name="ifPresent">the <see cref="Action{T}"/> to run if this <see cref="IOptional{T}.HasValue"/></param>
        /// <param name="orElse">the parameterless <see cref="Action"/> to run if this <see cref="IsEmpty{T}(BrandonUtils.Standalone.Optional.IOptional{T})"/></param>
        /// <typeparam name="TIn">the underlying type of this <see cref="Optional{T}"/></typeparam>
        public static void IfPresentOrElse<TIn>(this IOptional<TIn> optional, Action<TIn> ifPresent, Action orElse) {
            if (optional.HasValue) {
                ifPresent.Invoke(optional.Value);
            }
            else {
                orElse.Invoke();
            }
        }

        #endregion IfPresentOrElse

        #region OrElseThrow

        /// <summary>
        /// If this <see cref="IOptional{T}.HasValue"/>, returns <see cref="IOptional{T}.Value"/>.
        ///
        /// Throws an exception if the <see cref="IOptional{T}"/> is empty.
        /// </summary>
        /// <remarks>
        /// For the <see cref="Nullable{T}"/> equivalent, see <see cref="OrElseThrow{T}(Nullable{T},System.Func{System.Exception})"/>
        /// </remarks>
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
        /// <remarks>
        /// For the <see cref="Optional{T}"/> equivalent, see <see cref="OrElseThrow{T}(BrandonUtils.Standalone.Optional.IOptional{T},System.Func{System.Exception})"/>
        /// </remarks>
        /// <param name="nullable">a <see cref="Nullable{T}"/></param>
        /// <param name="exceptionProvider"><inheritdoc cref="OrElseThrow{T}(BrandonUtils.Standalone.Optional.IOptional{T},System.Func{System.Exception})"/></param>
        /// <typeparam name="T">the underlying type of the <see cref="Nullable{T}"/></typeparam>
        /// <returns></returns>
        /// <exception cref="Exception">if the <see cref="Nullable{T}"/> is empty</exception>
        [ContractAnnotation("nullable:null => stop")]
        public static T OrElseThrow<T>(this T? nullable, Func<Exception> exceptionProvider = default) where T : struct {
            return nullable ?? throw (exceptionProvider?.Invoke() ?? NoCanHasValue<T?>());
        }

        #endregion OrElseThrow

        private static ArgumentNullException NoCanHasValue<T>(T emptyThing = default) {
            return new ArgumentNullException($"The {typeof(T).Name} was empty!");
        }

        #region IsEmpty

        /// <returns>negation of <see cref="IOptional{T}.HasValue"/></returns>
        // public static bool IsEmpty<T>(this IOptional<T> optional) {
        //     return !optional.HasValue;
        // }

        /// <returns>negation of <see cref="Nullable{T}.HasValue"/></returns>
        public static bool IsEmpty<T>(this T? nullable) where T : struct {
            return !nullable.HasValue;
        }

        #endregion IsEmpty

        #region Function Fallbacks

        public static IOptional<TOut> FirstWithValue<TOut>(IEnumerable<Func<IOptional<TOut>>> functions) {
            return functions.Select(fn => fn.Invoke())
                            .FirstOrDefault(it => it.HasValue);
        }

        public static IOptional<TOut> FirstWithValue<TOut>(params Func<IOptional<TOut>>[] functions) {
            return FirstWithValue(functions as IEnumerable<Func<IOptional<TOut>>>);
        }

        public static IOptional<TOut> FirstWithValue<TIn, TOut>(TIn input, IEnumerable<Func<TIn, IOptional<TOut>>> functions) {
            return functions.Select(fn => fn.Invoke(input))
                            .FirstOrDefault(it => it.HasValue);
        }

        public static IOptional<TOut> FirstWithValue<TIn, TOut>(TIn input, params Func<TIn, IOptional<TOut>>[] functions) {
            return FirstWithValue(input, functions as IEnumerable<Func<TIn, IOptional<TOut>>>);
        }

        #endregion Function Fallbacks
    }
}