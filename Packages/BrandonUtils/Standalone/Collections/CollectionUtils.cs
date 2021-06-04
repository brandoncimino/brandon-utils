using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;

using BrandonUtils.Standalone.Enums;
using BrandonUtils.Standalone.Randomization;

namespace BrandonUtils.Standalone.Collections {
    /// <summary>
    ///     Contains utility and extension methods for collections, such as <see cref="IDictionary{TKey,TValue}" /> and <see cref="IList{T}" />.
    /// </summary>
    public static class CollectionUtils {
        #region Randomization

        /// <param name="collection"></param>
        /// <typeparam name="T">The type of the <see cref="Collection{T}"/></typeparam>
        /// <returns>a random <see cref="Enumerable.ElementAt{TSource}"/> from the given <paramref name="collection"/>.</returns>
        [Pure]
        public static T Random<T>(this ICollection<T> collection) {
            return collection.Count switch {
                1 => collection.Single(),
                0 => throw new IndexOutOfRangeException($"Attempted to select a {nameof(Random)} element, but the given collection was empty!"),
                _ => collection.ElementAt(Brandom.Gen.Next(0, collection.Count))
            };
        }

        /// <summary>
        /// Similar to <see cref="Random{T}"/>, but <b><see cref="Collection{T}.Remove"/>s the item</b>.
        /// </summary>
        /// <param name="collection"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns>a <see cref="Random{T}"/> entry from <paramref name="collection"/></returns>
        public static T GrabRandom<T>(this ICollection<T> collection) {
            var random = collection.Random();
            collection.Remove(random);
            return random;
        }

        /// <summary>
        /// Retrieves and <see cref="ICollection{T}.Remove"/>s the <see cref="Enumerable.ElementAt{TSource}"/> <paramref name="index"/>
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="index"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Grab<T>(this ICollection<T> collection, int index) {
            var item = collection.ElementAt(index);
            collection.Remove(item);
            return item;
        }

        /// <summary>
        /// Randomizes all of the entries in <see cref="oldList"/>.
        /// </summary>
        /// <param name="oldList"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static void Randomize<T>(this ICollection<T> oldList) {
            var backupList = oldList.Copy();
            oldList.Clear();

            while (backupList.Any()) {
                oldList.Add(GrabRandom(backupList));
            }

            // Previously, I was returning the collection, for method chaining; but I couldn't get the generics to be happy about that :(
            // Having this be void makes me sad, but it's the same as .Sort() :(
            // return oldList;
        }

        /// <summary>
        /// TODO: I would like it if this wasn't limited to <see cref="List{T}"/>, but that would require 2 type parameters...
        /// </summary>
        /// <param name="oldList"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [Pure]
        public static IList<T> RandomCopy<T>(this List<T> oldList) {
            var copy = oldList.Copy();
            copy.Randomize();
            return copy;
        }

        [Pure]
        // ReSharper disable once ReturnTypeCanBeEnumerable.Global
        public static List<T> ListOf<T>(params T[] stuff) {
            return new List<T>(stuff);
        }

        #endregion

        #region Copying

        public static IList<T> Copy<T>(this IList<T> oldCollection) {
            return oldCollection.Select(it => it).ToList();
        }

        public static ICollection<T> Copy<T>(this ICollection<T> oldCollection) {
            return oldCollection.Select(it => it).ToList();
        }

        public static IEnumerable<T> Copy<T>(this IEnumerable<T> oldList) {
            return oldList.Select(it => it);
        }

        /// <summary>
        /// Splits <paramref name="collection"/> into multiple <see cref="List{T}"/>s, whose sizes are determined by <paramref name="subGroups"/>.
        /// </summary>
        /// <param name="collection">The original <see cref="ICollection{T}"/></param>
        /// <param name="subGroups">The size of each resulting <see cref="List{T}"/>, in order</param>
        /// <typeparam name="T">The <see cref="Type"/> <typeparamref name="T"/> of <paramref name="collection"/></typeparam>
        /// <returns>A jagged <see cref="List{T}"/> of <see cref="List{T}"/>s, containing all of the elements of <paramref name="collection"/>, in order.</returns>
        /// <exception cref="Enumerable.Sum(System.Collections.Generic.IEnumerable{int})">if the <see cref="Enumerable"/> of <paramref name="subGroups"/> does not equal the <see cref="ICollection{T}"/> of <paramref name="collection"/></exception>
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

        #region Dictionary Inversion

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
        public static Dictionary<TValue_Original, TKey_Original> Inverse<TKey_Original, TValue_Original>(this Dictionary<TKey_Original, TValue_Original> dictionary) {
            return (Dictionary<TValue_Original, TKey_Original>) Inverse_Internal(dictionary);
        }

        /// <inheritdoc cref="Inverse_Internal{TKey_Original,TValue_Original}"/>
        public static ReadOnlyDictionary<TValue_Original, TKey_Original> Inverse<TKey_Original, TValue_Original>(this ReadOnlyDictionary<TKey_Original, TValue_Original> readOnlyDictionary) {
            return (ReadOnlyDictionary<TValue_Original, TKey_Original>) Inverse_Internal(readOnlyDictionary);
        }

        #endregion

        #endregion

        /// <summary>
        ///     Similarly to <see cref="List{T}.ForEach" />, this performs <paramref name="action" /> against each
        ///     <b>
        ///         <see cref="KeyValuePair{TKey,TValue}" />
        ///     </b>
        ///     in <paramref name="dictionary" />.
        /// </summary>
        /// <example>
        ///     Which variation of <see cref="ForEach{T,T2}(System.Collections.Generic.IDictionary{T,T2},System.Action{System.Collections.Generic.KeyValuePair{T,T2}})">ForEach</see> is called depends on the first time the <paramref name="action" />'s <see cref="Delegate.Target" /> parameter is accessed.
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
        /// <param name="dictionary">The <see cref="IDictionary{TKey,TValue}" /> you would like to iterate over.</param>
        /// <param name="action">The <see cref="Action" /> that will be performed against each <see cref="KeyValuePair{TKey,TValue}" /> in <paramref name="dictionary" />.</param>
        /// <typeparam name="T">The type of <paramref name="dictionary" />'s <see cref="IDictionary{TKey,TValue}.Keys" /></typeparam>
        /// <typeparam name="T2">The type of <paramref name="dictionary" />'s <see cref="IDictionary{TKey,TValue}.Values" /></typeparam>
        /// <seealso cref="ForEach{T,T2}(System.Collections.Generic.IDictionary{T,T2},System.Action{T2})" />
        /// <seealso cref="List{T}.ForEach" />
        public static void ForEach<T, T2>(this IDictionary<T, T2> dictionary, Action<KeyValuePair<T, T2>> action) {
            foreach (var pair in dictionary) {
                action.Invoke(pair);
            }
        }

        /// <inheritdoc cref="ForEach{T,T2}(System.Collections.Generic.IDictionary{T,T2},System.Action{System.Collections.Generic.KeyValuePair{T,T2}})"/>
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

        public static string JoinLines<T>(this IEnumerable<T> enumerable) {
            return string.Join("\n", enumerable);
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

        #region Dictionary Joining

        /// <summary>
        /// Removes all of the <paramref name="keysToRemove"/> from <paramref name="original"/> via <see cref="IDictionary{TKey,TValue}.Remove(TKey)"/>.
        /// </summary>
        /// <remarks>
        /// An error is <b>not</b> thrown when a key from <paramref name="keysToRemove"/> isn't found in <paramref name="original"/>.
        /// </remarks>
        /// <param name="original"></param>
        /// <param name="keysToRemove"></param>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public static IDictionary<TKey, TValue> RemoveAll<TKey, TValue>(this IDictionary<TKey, TValue> original, IEnumerable<TKey> keysToRemove) {
            foreach (var key in keysToRemove) {
                original.Remove(key);
            }

            return original;
        }

        /// <summary>
        /// Calls <see cref="RemoveAll{TKey,TValue}(System.Collections.Generic.IDictionary{TKey,TValue},System.Collections.Generic.IEnumerable{TKey})"/> using <paramref name="IDictionaryWithKeysToRemove"/>'s <see cref="IDictionary{TKey,TValue}.Keys"/>.
        /// </summary>
        /// <param name="original"></param>
        /// <param name="dictionaryWithKeysToRemove"></param>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public static IDictionary<TKey, TValue> RemoveAll<TKey, TValue>(this IDictionary<TKey, TValue> original, IDictionary<TKey, TValue> dictionaryWithKeysToRemove) {
            return RemoveAll(original, dictionaryWithKeysToRemove.Keys);
        }

        /// <summary>
        /// The method by which <see cref="CollectionUtils.JoinDictionaries{TKey,TValue}"/> will handle conflicts caused by duplicate <see cref="IDictionary{TKey,TValue}.Keys"/>.
        /// </summary>
        public enum ConflictResolution {
            /// <summary>
            /// If a duplicate <see cref="KeyValuePair{TKey,TValue}.Key"/> is found, an exception will be thrown.
            /// </summary>
            Fail,
            /// <summary>
            /// If a duplicate <see cref="KeyValuePair{TKey,TValue}.Key"/> is found, prefer to use the original's <see cref="KeyValuePair{TKey,TValue}.Value"/>.
            /// </summary>
            FavorOriginal,
            /// <summary>
            /// If a duplicate <see cref="KeyValuePair{TKey,TValue}.Key"/> is found, prefer to use the new <see cref="KeyValuePair{TKey,TValue}.Value"/>.
            /// </summary>
            FavorNew
        }

        /// <summary>
        /// Returns a <see cref="IDictionary{TKey,TValue}"/> containing:
        /// <li>The <see cref="IDictionary{TKey,TValue}.Keys"/> that exist in both <paramref name="original"/> and <paramref name="additional"/>.</li>
        /// <li>The <see cref="IDictionary{TKey,TValue}.Values"/> for those keys from either <paramref name="original"/> or <paramref name="additional"/>, depending on <paramref name="conflictResolution"/>.</li>
        /// </summary>
        /// <remarks>
        /// The <see cref="IDictionary{TKey,TValue}.Keys"/> of the result will always be in the order they appeared <b>inside of <paramref name="original"/></b>, regardless of <paramref name="conflictResolution"/>.
        /// </remarks>
        /// <param name="original"></param>
        /// <param name="additional"></param>
        /// <param name="conflictResolution">The <see cref="ConflictResolution"/> to decide which <see cref="IDictionary{TKey,TValue}.Values"/> are returned.</param>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        /// <exception cref="ConflictResolution.Fail">If <see cref="ConflictResolution"/> or an unknown <see cref="InvalidEnumArgumentException"/> is passed.</exception>
        [Pure]
        public static Dictionary<TKey, TValue> Overlap<TKey, TValue>(this IDictionary<TKey, TValue> original, IDictionary<TKey, TValue> additional, ConflictResolution conflictResolution) {
            var overlappingKeys = original.Keys.Where(additional.ContainsKey);

            switch (conflictResolution) {
                case ConflictResolution.Fail:
                    throw new InvalidEnumArgumentException($"When specifically requesting {nameof(Overlap)}, you can't have a {nameof(ConflictResolution)} method of {ConflictResolution.Fail}! How would that make any sense?");
                case ConflictResolution.FavorOriginal:
                    return overlappingKeys.ToDictionary(key => key, key => original[key]);
                case ConflictResolution.FavorNew:
                    return overlappingKeys.ToDictionary(key => key, key => additional[key]);
                default:
                    throw EnumUtils.InvalidEnumArgumentException(nameof(ConflictResolution), conflictResolution);
            }
        }

        /// <summary>
        /// Updates any <see cref="IDictionary{TKey,TValue}.Keys"/> in <paramref name="original"/> with their <see cref="KeyValuePair{TKey,TValue}.Value"/> from <paramref name="newValues"/>.
        /// <p/>
        /// <see cref="IDictionary{TKey,TValue}.Keys"/> found in <paramref name="newValues"/> but <b>not</b> in <paramref name="original"/> are ignored.
        /// </summary>
        /// <param name="original"></param>
        /// <param name="newValues"></param>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public static IDictionary<TKey, TValue> UpdateFrom<TKey, TValue>(this IDictionary<TKey, TValue> original, IDictionary<TKey, TValue> newValues) {
            newValues.ForEach(
                pair => {
                    if (original.ContainsKey(pair.Key)) {
                        original[pair.Key] = pair.Value;
                    }
                }
            );
            return original;
        }

        /// <summary>
        /// Joins <b>all</b> of the given <paramref name="dictionaries"/> together via the given <see cref="ConflictResolution"/> method, returning a <b>new <see cref="IDictionary{TKey,TValue}"/></b>.
        /// </summary>
        /// <param name="dictionaries"></param>
        /// <param name="conflictResolution"></param>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentException">If <paramref name="conflictResolution"/> is <see cref="ConflictResolution.Fail"/> and there are duplicate <see cref="IDictionary{TKey,TValue}.Keys"/> in the <paramref name="dictionaries"/>.</exception>
        /// <exception cref="InvalidEnumArgumentException"></exception>
        [Pure]
        public static Dictionary<TKey, TValue> JoinDictionaries<TKey, TValue>(
            IEnumerable<IDictionary<TKey, TValue>> dictionaries,
            ConflictResolution conflictResolution = ConflictResolution.Fail
        ) {
            var allKeys = dictionaries.SelectMany(dic => dic.Keys);

            if (!allKeys.IsSingleton()) {
                switch (conflictResolution) {
                    case ConflictResolution.Fail:
                        throw new ArgumentException($"Could not join the dictionaries because they had duplicate keys and the {nameof(conflictResolution)} method was set to {ConflictResolution.Fail}");
                    case ConflictResolution.FavorNew:
                        dictionaries = dictionaries.Reverse();
                        break;
                    case ConflictResolution.FavorOriginal:
                        //don't need to do nu'n
                        break;
                    default:
                        throw EnumUtils.InvalidEnumArgumentException(nameof(conflictResolution), conflictResolution);
                }
            }

            return allKeys.Distinct().ToDictionary(key => key, dictionaries.FirstNonEmptyValue);
        }

        /// <summary>
        /// Joins <paramref name="original"/> and <paramref name="additional"/> together via the given <see cref="ConflictResolution"/> method, returning a <b>new <see cref="IDictionary{TKey,TValue}"/></b>.
        /// </summary>
        /// <remarks>
        /// The order of the <see cref="IDictionary{TKey,TValue}.Keys"/> in the result will always be:
        /// <li><b>All</b> of the <see cref="IDictionary{TKey,TValue}.Keys"/> from <paramref name="original"/> (including any overlap).</li>
        /// <li>The <b>unique</b> <see cref="IDictionary{TKey,TValue}.Keys"/> from <see cref="additional"/>.</li>
        /// </remarks>
        /// <param name="original"></param>
        /// <param name="additional"></param>
        /// <param name="conflictResolution"></param>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentException">If <paramref name="conflictResolution"/> is <see cref="ConflictResolution.Fail"/> and there are duplicate <see cref="IDictionary{TKey,TValue}.Keys"/> between <paramref name="original"/> and <paramref name="additional"/>.</exception>
        /// <exception cref="InvalidEnumArgumentException"></exception>
        [Pure]
        public static IDictionary<TKey, TValue> JoinDictionaries<TKey, TValue>(
            this IDictionary<TKey, TValue> original,
            IDictionary<TKey, TValue> additional,
            ConflictResolution conflictResolution = ConflictResolution.Fail
        ) {
            return JoinDictionaries(new[] {original, additional}, conflictResolution);
        }

        [Pure]
        public static IDictionary<TKey, TValue> JoinDictionaries<TKey, TValue>(
            this IDictionary<TKey, TValue> original,
            params IDictionary<TKey, TValue>[] additional
        ) {
            return JoinDictionaries(additional.Prepend(original));
        }

        [Pure]
        public static TValue FirstValue<TKey, TValue>(
            this IEnumerable<Dictionary<TKey, TValue>> dictionaries,
            TKey key
        ) {
            return dictionaries.First(dic => dic.ContainsKey(key))[key];
        }

        [Pure]
        public static TValue FirstNonEmptyValue<TKey, TValue>(
            this IEnumerable<IDictionary<TKey, TValue>> dictionaries,
            TKey key
        ) {
            return dictionaries.First(dic => dic.ContainsKey(key) && ReflectionUtils.IsNotEmpty(dic[key]))[key];
        }

        /// <summary>
        /// A simplified version of <see cref="Enumerable.GroupBy{TSource,TKey}(System.Collections.Generic.IEnumerable{TSource},System.Func{TSource,TKey})"/> that groups entries by equality.
        /// </summary>
        /// <param name="enumerable"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [Pure]
        public static IDictionary<T, int> Group<T>(this IEnumerable<T> enumerable) {
            return enumerable.GroupBy(it => it).ToDictionary(it => it.Key, it => it.Count());
        }

        #endregion
    }
}
