using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BrandonUtils.Standalone {
    public static class Bloop {
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action) {
            foreach (var e in enumerable) {
                action.Invoke(e);
            }
        }

        #region "Repeat" Extensions

        #region int.Repeat(lambda) NOTE: These are the "real" or "base" methods; the rest of these methods are aliases for these!
        /// <summary>
        /// Similar to <see cref="Enumerable.Repeat{TResult}"/>, but evaluates <paramref name="supplier"/> separately for each iteration.
        /// </summary>
        /// <example>
        /// At first I wanted to write:
        ///
        /// <code><![CDATA[
        /// int Total = 0;
        /// void Counter() => Total++;
        /// List<int> Numbers = Enumerable.Repeat(Counter(), 5);
        /// ]]></code>
        ///
        /// Thinking that it would produce <c>{0,1,2,3,4}</c>.
        /// <br/>
        /// However, <c>Counter()</c> will only be evaluated <b>once</b>, and then have <b>that first result</b> repeated,
        /// producing <c>{0,0,0,0,0}</c>.
        /// <br/>
        /// Using <see cref="Repeat{T}(int,System.Func{T})"/> the contents will be evaluated "lazily", once per entry:
        ///
        /// <code><![CDATA[
        /// int Total = 0;
        /// void Counter() => Total++;
        /// List<int> Numbers = Bloop.Repeat(5, () => Counter());
        /// ]]></code>
        /// Will produce <c>{0,1,2,3,4,5}</c>.
        /// </example>
        /// <remarks>
        /// The name "<paramref name="supplier"/>" comes from Java's <a href="https://docs.oracle.com/javase/8/docs/api/java/util/function/Supplier.html">Supplier</a>.
        /// </remarks>
        /// <param name="count">the number of times to execute the <see cref="Bloop"/></param>
        /// <param name="supplier">a <see cref="Func{TResult}"/> that takes no input and produces an instance of <typeparamref name="T"/></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> Repeat<T>(this int count, Func<T> supplier) {
            return Enumerable.Repeat(supplier, count).Select(it => it.Invoke());
        }

        /// <summary>
        /// Executes <paramref name="consumer"/> <paramref name="count"/> times, passing the <b>current iteration</b> to <paramref name="consumer"/> each time.
        /// </summary>
        /// <example>
        /// This is essentially the same as a traditional "for" loop, but can be written much more concisely and, I think, intuitively:
        ///
        /// <code><![CDATA[
        /// for(int i=0; i<10; i++){
        ///     Yolo(i);
        /// }
        /// ]]></code>
        ///
        /// Becomes:
        ///
        /// <code><![CDATA[
        /// Bloop.For(10, i => Yolo(i));
        /// ]]></code>
        ///
        /// Or, if you're feeling <i>really</i> frisky:
        ///
        /// <code><![CDATA[
        /// 10.For(i => Yolo(i))
        /// ]]></code>
        /// </example>
        /// <param name="count"></param>
        /// <param name="consumer"></param>
        public static void Repeat(this int count, Action<int> consumer) {
            for (int i = 0; i < count; i++) {
                consumer.Invoke(i);
            }
        }

        /// <summary>
        /// A combination of <see cref="Repeat{T}(int,System.Func{T})"/> and <see cref="Repeat(int,System.Action{int})"/>:
        /// <ul>
        /// <li>Executes <paramref name="loopFunction"/> once per iteration</li>
        /// <li>Passes the current iteration number to <paramref name="loopFunction"/></li>
        /// </ul>
        /// </summary>
        /// <param name="count"></param>
        /// <param name="loopFunction"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> Repeat<T>(this int count, Func<int, T> loopFunction) {
            return Enumerable.Range(0, count).Select(loopFunction.Invoke);
        }

        /// <summary>
        /// Invokes <paramref name="action"/> <paramref name="count"/> times, without returning any results.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="action"></param>
        public static void Repeat(this int count, Action action) {
            for (int i = 0; i < count; i++) {
                action.Invoke();
            }
        }
        #endregion

        #region int.For(lambda)

        /// <inheritdoc cref="Repeat{T}(int,System.Func{T})"/>
        public static IEnumerable<T> For<T>(this int iterations, Func<T> supplier) {
            return iterations.Repeat(supplier);
        }

        /// <inheritdoc cref="Repeat{T}(int,System.Func{int,T})"/>
        public static IEnumerable<T> For<T>(this int iterations, Func<int, T> loopFunction) {
            return iterations.Repeat(loopFunction);
        }

        /// <inheritdoc cref="Repeat(int,Action)"/>
        public static void For(this int iterations, Action action) {
            iterations.Repeat(action);
        }

        /// <inheritdoc cref="Repeat(int,Action{int})"/>
        public static void For(this int iterations, Action<int> consumer) {
            iterations.Repeat(consumer);
        }
        #endregion

        #region lambda.Repeat(int)

        /// <inheritdoc cref="Repeat{T}(int, Func{T})"/>
        public static IEnumerable<T> Repeat<T>(this Func<T> supplier, int count) {
            return count.Repeat(supplier);
        }

        /// <inheritdoc cref="Repeat{T}(int,Func{int,T})"/>
        public static IEnumerable<T> Repeat<T>(this Func<int, T> loopFunction, int count) {
            return count.Repeat(loopFunction);
        }

        /// <inheritdoc cref="Repeat(int,Action)"/>
        public static void Repeat(this Action action, int count) {
            count.Repeat(action);
        }

        /// <inheritdoc cref="Repeat(int, Action{int})"/>
        public static void Repeat(this Action<int> consumer, int count) {
            count.Repeat(consumer);
        }

        #endregion

        #region lambda.For(int)

        /// <inheritdoc cref="Repeat{T}(int,System.Func{T})"/>
        public static IEnumerable<T> For<T>(this Func<T> supplier, int count) {
            return count.Repeat(supplier);
        }

        /// <inheritdoc cref="Repeat{T}(int,System.Func{int,T})"/>
        public static IEnumerable<T> For<T>(this Func<int, T> loopFunction, int count) {
            return count.Repeat(loopFunction);
        }

        /// <inheritdoc cref="Repeat(int,Action)"/>
        public static void For(this Action action, int count) {
            count.Repeat(action);
        }

        /// <inheritdoc cref="Repeat(int,Action{int})"/>
        public static void For(this Action<int> action, int count) {
            count.Repeat(action);
        }

        #endregion

        #endregion
    }
}
