using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using BrandonUtils.Standalone.Collections;
using BrandonUtils.Standalone.Exceptions;
using BrandonUtils.Standalone.Reflection;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Strings.Prettifiers {
    internal static partial class InnerPretty {
        private static Type InferType(IEnumerable<object> stuff) {
            try {
                return ReflectionUtils.CommonType(stuff.Select(it => it.GetType()));
            }
            catch {
                return typeof(object);
            }
        }

        /// <summary>
        /// TODO: Update this to work with an arbitrary number of columns
        /// </summary>
        /// <param name="dictionary">the <see cref="IDictionary"/> to be prettified</param>
        /// <param name="settings">optional <see cref="PrettificationSettings"/></param>
        /// <returns>a pretty <see cref="string"/></returns>
        public static string PrettifyDictionary(IDictionary dictionary, PrettificationSettings settings = default) {
            var keys = dictionary.Keys.Cast<object>().ToList();
            var vals = dictionary.Values.Cast<object>().ToList();

            return PrettifyDictionary(keys, vals, settings);
        }

        private static string PrettifyDictionary(IList<object> keys, IList<object> vals, PrettificationSettings settings) {
            // get the types of the columns, to use as headers
            var keyType = InferType(keys);
            var valType = InferType(vals);
            return PrettifyDictionary(keys, vals, keyType, valType, settings);
        }

        /// <summary>
        /// The basis of the <see cref="InnerPretty"/> methods for dictionaries.
        /// </summary>
        /// <remarks>
        /// Rather than operating on an actual <see cref="IDictionary{TKey,TValue}"/>, this takes in two <see cref="List{T}"/>s and explicit column headers.
        /// </remarks>
        /// <param name="keys"></param>
        /// <param name="vals"></param>
        /// <param name="keyHeader"></param>
        /// <param name="valHeader"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="BrandonException"></exception>
        private static string PrettifyDictionary(
            [NotNull, ItemCanBeNull]
            IList<object> keys,
            [NotNull, ItemCanBeNull]
            IList<object> vals,
            [NotNull]   object                 keyHeader,
            [NotNull]   object                 valHeader,
            [CanBeNull] PrettificationSettings settings
        ) {
            settings ??= Prettification.DefaultPrettificationSettings;

            // add the headers
            keys.Insert(0, keyHeader);
            vals.Insert(0, valHeader);

            var keyLines = keys.Select(it => it.Prettify(LineStyle.Single)).ToList();
            var valLines = vals.Select(it => it.Prettify(LineStyle.Single)).ToList();

            const string headerSeparatorChar = "-";

            if (keyLines == null) {
                throw new NullReferenceException($"{nameof(keyLines)} is null");
            }

            if (valLines == null) {
                throw new NullReferenceException($"{nameof(valLines)} is null");
            }

            var unlimitedKeyWidth = keyLines.LongestLine();
            var unlimitedValWidth = valLines.LongestLine();

            int keyWidth;
            int valWidth;
            if (unlimitedKeyWidth + unlimitedValWidth < settings.LineLengthLimit) {
                keyWidth = unlimitedKeyWidth;
                valWidth = unlimitedValWidth;
            }
            else {
                var   totalWidth    = unlimitedKeyWidth + unlimitedValWidth;
                float keyWidthRatio = (float)unlimitedKeyWidth / totalWidth;
                float valWidthRatio = (float)unlimitedValWidth / totalWidth;
                var   exactKeyWidth = totalWidth               * keyWidthRatio;
                var   exactValWidth = totalWidth               * valWidthRatio;
                if (exactKeyWidth < exactValWidth) {
                    keyWidth = exactKeyWidth.CeilingToInt();
                    valWidth = exactValWidth.FloorToInt();
                }
                else {
                    keyWidth = exactKeyWidth.FloorToInt();
                    valWidth = exactValWidth.CeilingToInt();
                }

                // checking my work
                if (keyWidth + valWidth != totalWidth) {
                    throw new BrandonException($"[{nameof(keyWidth)}] {keyWidth} + [{nameof(valWidth)}] {valWidth} != [{totalWidth}] {totalWidth}!");
                }
            }

            // add the separators
            keyLines.Insert(1, headerSeparatorChar.Repeat(keyWidth));
            valLines.Insert(1, headerSeparatorChar.Repeat(valWidth));

            //NOTE: There is actually a version of select that uses the index! O_O
            var lines = keyLines.Select((it, i) => PrettifyPair(it, valLines[i], keyWidth, valWidth))
                                .Bookend("\n")
                                .ToList();
            return lines.JoinLines();
        }

        private static string PrettifyPair(object a, object b, int aWidth, int bWidth, string separator = " ") {
            return $"{PrettifyCell(a, aWidth)}{separator}{PrettifyCell(b, bWidth)}";
        }

        private static string PrettifyCell(object cellValue, int columnWidth) {
            var str = cellValue.Prettify(LineStyle.Single);
            return str.ForceToLength(columnWidth);
        }

        /**
         * <remarks>I would have this operate on <see cref="KeyedCollection{TKey,TItem}"/>, but unfortunately, <see cref="KeyedCollection{TKey,TItem}.GetKeyForItem"/> is <c>protected</c>.</remarks>
         */
        internal static string PrettifyKeyedList<TKey, TValue>(KeyedList<TKey, TValue> keyedList, PrettificationSettings settings = default) {
            IDictionary dictionary = keyedList.ToDictionary();
            return PrettifyDictionary(
                dictionary.Keys.Cast<object>().ToList(),
                dictionary.Values.Cast<object>().ToList(),
                typeof(TKey),
                typeof(TValue),
                settings
            );
        }

        /// <summary>
        /// The generic version of <see cref="PrettifyDictionary"/>
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="settings"></param>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        internal static string PrettifyDictionary<TKey, TValue>(IDictionary<TKey, TValue> dictionary, PrettificationSettings settings = default) {
            return PrettifyDictionary(
                dictionary.Keys.Cast<object>().ToList(),
                dictionary.Values.Cast<object>().ToList(),
                typeof(TKey),
                typeof(TValue),
                settings
            );
        }
    }
}