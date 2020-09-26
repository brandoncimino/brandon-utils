using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using JetBrains.Annotations;

namespace Packages.BrandonUtils.Runtime.Collections {
    /// <summary>
    ///     Contains utility and extension methods for collections, such as <see cref="IList{T}" /> and <see cref="IDictionary{TKey,TValue}" />.
    /// </summary>
    public static class CollectionUtils {
        public static T Random<T>(this IEnumerable<T> enumerable) {
            var array = enumerable as T[] ?? enumerable.ToArray();
            return array.ElementAt(new System.Random().Next(0, array.Length));
        }

        public static T GrabRandom<T>(this ICollection<T> list) {
            var random = list.Random();
            list.Remove(random);
            return random;
        }

        public static ICollection<T> Randomize<T>(this ICollection<T> oldList) {
            var backupList = oldList.Copy();
            oldList.Clear();

            while (backupList.Any()) {
                oldList.Add(GrabRandom(backupList));
            }

            return oldList;
        }

        public static ICollection<T> RandomCopy<T>(this ICollection<T> oldList) {
            return oldList.Copy().Randomize();
        }

        public static IList<T> Copy<T>(this IList<T> oldCollection) {
            return oldCollection.Select(it => it).ToList();
        }

        public static ICollection<T> Copy<T>(this ICollection<T> oldCollection) {
            return (ICollection<T>) oldCollection.Select(it => it);
        }

        public static IEnumerable<T> Copy<T>(this IEnumerable<T> oldList) {
            return oldList.Select(it => it).ToList();
        }

        /// <summary>
        ///     Similarly to <see cref="List{T}.ForEach" />, this performs <paramref name="action" /> against each
        ///     <b>
        ///         <see cref="KeyValuePair{TKey,TValue}" />
        ///     </b>
        ///     in <paramref name="dictionary" />.
        /// </summary>
        /// <example>
        ///     Which variation of <see cref="ForEach{T,T2}(System.Collections.Generic.Dictionary{T,T2},System.Action{System.Collections.Generic.KeyValuePair{T,T2}})">ForEach</see> is called depends on the first time the <paramref name="action" />'s <see cref="Delegate.Target" /> parameter is accessed.
        /// <p />For example, given:
        /// <code><![CDATA[
        /// Dictionary dictionary = new Dictionary<string, string>();
        /// ]]></code>
        /// Then the following would treat <b><i><c>it</c></i></b> as a <see cref="KeyValuePair{TKey,TValue}" />:
        /// <code><![CDATA[
        /// dictionary.ForEach(it => Console.WriteLine(it.Key);
        /// ]]></code>
        /// While the following would treat <b><i><c>it</c></i></b> as a <see cref="string" />:
        /// <code><![CDATA[
        /// dictionary.ForEach(it => Console.WriteLine(it.Length));
        /// ]]></code>
        /// While the following would <b>fail to compile</b>, due to attempting to treating <c>it</c> as both a <see cref="KeyValuePair{TKey,TValue}" /> and a <see cref="string" />:
        /// <code><![CDATA[
        /// dictionary.ForEach(it => Console.WriteLine($"{it.Key}, {it.Length}"));
        /// ]]></code>
        /// And the following would <b>fail to compile</b>, due to being ambiguous:
        /// <code><![CDATA[
        /// dictionary.ForEach(it => Console.WriteLine(it));
        /// ]]></code>
        /// </example>
        /// <param name="dictionary">The <see cref="Dictionary{TKey,TValue}" /> you would like to iterate over.</param>
        /// <param name="action">The <see cref="Action" /> that will be performed against each <see cref="KeyValuePair{TKey,TValue}" /> in <paramref name="dictionary" />.</param>
        /// <typeparam name="T">The type of <paramref name="dictionary" />'s <see cref="Dictionary{TKey,TValue}.Keys" /></typeparam>
        /// <typeparam name="T2">The type of <paramref name="dictionary" />'s <see cref="Dictionary{TKey,TValue}.Values" /></typeparam>
        /// <seealso cref="ForEach{T,T2}(System.Collections.Generic.IDictionary{T,T2},System.Action{T2})" />
        /// <seealso cref="List{T}.ForEach" />
        public static void ForEach<T, T2>(this IDictionary<T, T2> dictionary, Action<KeyValuePair<T, T2>> action) {
            foreach (var pair in dictionary) {
                action.Invoke(pair);
            }
        }

        /// <inheritdoc cref="ForEach{T,T2}(System.Collections.Generic.Dictionary{T,T2},System.Action{System.Collections.Generic.KeyValuePair{T,T2}})" />
        /// <summary>
        ///     Similarly to <see cref="List{T}.ForEach" />, this performs <paramref name="action" /> against each
        ///     <b>
        ///         <see cref="KeyValuePair{TKey,TValue}.Value" />
        ///     </b>
        ///     in <paramref name="dictionary" />.
        /// </summary>
        public static void ForEach<T, T2>(this IDictionary<T, T2> dictionary, Action<T2> action) {
            dictionary.Values.ToList().ForEach(action);
        }

        /// <summary>
        /// Calls <see cref="string.Join(string,System.Collections.Generic.IEnumerable{string})"/> against <paramref name="enumerable"/>.
        /// </summary>
        /// <remarks>Corresponds roughly to Java's <a href="https://docs.oracle.com/javase/8/docs/api/java/util/stream/Collectors.html#joining--">.joining()</a> collector.
        /// </remarks>
        /// <param name="enumerable"></param>
        /// <param name="separator"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string JoinString<T>(this IEnumerable<T> enumerable, string separator = "") {
            return string.Join(separator, enumerable);
        }

        /// <summary>
        /// Returns true if <paramref name="enumerable"/> contains only unique elements, as determined via <see cref="Enumerable.Distinct{TSource}(System.Collections.Generic.IEnumerable{TSource})"/>
        /// </summary>
        /// <param name="enumerable"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool IsSingleton<T>(this IEnumerable<T> enumerable) {
            var enumerable1 = enumerable as T[] ?? enumerable.ToArray();
            return enumerable1.Length == enumerable1.Distinct().Count();
        }

        [NotNull]
        public static IDictionary<TValue_Original, TKey_Original> Inverse_Internal<TKey_Original, TValue_Original>(IDictionary<TKey_Original, TValue_Original> dictionary) {
            //to make for specific and explicit error messages, we check for known failures conditions ahead of time - specifically, duplicate keys results in an "ArgumentException" which is the parent class of lots of other things, making it not very clear when we catch it...
            if (!dictionary.Values.IsSingleton()) {
                throw new ArgumentException($"The provided {dictionary.GetType().Name}'s {nameof(dictionary.Values)} contained one or more duplicates, so they couldn't be used as keys!");
            }

            if (dictionary.Any(pair => pair.Value == null)) {
                throw new ArgumentNullException($"The provided {dictionary.GetType().Name} contained a null {nameof(KeyValuePair<object, object>.Value)}, which can't be used as a {nameof(KeyValuePair<object, object>.Key)}!");
            }

            var inverted = dictionary.ToDictionary(pair => pair.Value, pair => pair.Key);

            if (dictionary is ReadOnlyDictionary<TKey_Original, TValue_Original>) {
                return new ReadOnlyDictionary<TValue_Original, TKey_Original>(inverted);
            }
            else {
                return new Dictionary<TValue_Original, TKey_Original>(inverted);
            }
        }

        /// <inheritdoc cref="Inverse_Internal{TKey_Original,TValue_Original}"/>
        [NotNull]
        public static Dictionary<TValue_Original, TKey_Original> Inverse<TKey_Original, TValue_Original>(this Dictionary<TKey_Original, TValue_Original> dictionary) {
            return (Dictionary<TValue_Original, TKey_Original>) Inverse_Internal(dictionary);
        }

        /// <inheritdoc cref="Inverse_Internal{TKey_Original,TValue_Original}"/>
        [NotNull]
        public static ReadOnlyDictionary<TValue_Original, TKey_Original> Inverse<TKey_Original, TValue_Original>(this ReadOnlyDictionary<TKey_Original, TValue_Original> readOnlyDictionary) {
            return (ReadOnlyDictionary<TValue_Original, TKey_Original>) Inverse_Internal(readOnlyDictionary);
        }

        /// <summary>
        /// Splits <paramref name="collection"/> into multiple <see cref="List{T}"/>s, whose sizes are determined by <paramref name="subGroups"/>.
        /// </summary>
        /// <param name="collection">The original <see cref="ICollection{T}"/></param>
        /// <param name="subGroups">The size of each resulting <see cref="List{T}"/>, in order</param>
        /// <typeparam name="T">The <see cref="Type"/> <typeparamref name="T"/> of <paramref name="collection"/></typeparam>
        /// <returns>A jagged <see cref="List{T}"/> of <see cref="List{T}"/>s, containing all of the elements of <paramref name="collection"/>, in order.</returns>
        /// <exception cref="ArgumentOutOfRangeException">if the <see cref="Enumerable.Sum(System.Collections.Generic.IEnumerable{int})"/> of <paramref name="subGroups"/> does not equal the <see cref="ICollection{T}.Count"/> of <paramref name="collection"/></exception>
        public static List<List<T>> SplitCollection<T>(this ICollection<T> collection, ICollection<int> subGroups) {
            if (subGroups.Sum() != collection.Count) {
                throw new ArgumentOutOfRangeException(
                    nameof(subGroups),
                    $"The provided sub-groups do not total ({subGroups.Sum()}) the length of the collection ({collection.Count})!"
                );
            }

            var splitGroups  = new List<List<T>>();
            int currentIndex = 0;
            foreach (var subGroupSize in subGroups) {
                splitGroups.Add(collection.ToList().GetRange(currentIndex, subGroupSize));
                currentIndex += subGroupSize;
            }

            return splitGroups;
        }

        /// <summary>
        /// Computes the element-wise difference between each element of <paramref name="listToCompute"/>.
        /// </summary>
        /// <remarks>
        /// Each element in <paramref name="listToCompute"/> is subtracted from the element <b>following it</b>, i.e.:
        /// <code><![CDATA[
        /// Input:             [3, 1, 4]
        /// Calculations:      1-3   4-1
        /// Result:             [2, 3]
        /// ]]></code>
        /// </remarks>
        /// <param name="listToCompute"></param>
        /// <returns>A <see cref="List{T}"/> (with <see cref="List{T}.Count"/> equal to <paramref name="listToCompute"/>.<see cref="List{T}.Count"/> - 1) containing the pair-wise difference between each element of <paramref name="listToCompute"/></returns>
        public static List<int> GetListDiff(this List<int> listToCompute) {
            var diff = new List<int>();
            for (int i = 0; i < listToCompute.Count - 1; i++) {
                diff.Add(listToCompute[i + 1] - listToCompute[i]);
            }

            return diff;
        }
    }
}