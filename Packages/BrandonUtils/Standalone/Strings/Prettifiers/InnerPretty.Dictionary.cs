using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;

using BrandonUtils.Standalone.Collections;

namespace BrandonUtils.Standalone.Strings.Prettifiers {
    internal static partial class InnerPretty {
        /// <summary>
        /// TODO: Update this to work with an arbitrary number of columns
        /// </summary>
        /// <param name="dictionary">the <see cref="IDictionary"/> to be prettified</param>
        /// <param name="settings">optional <see cref="PrettificationSettings"/></param>
        /// <returns>a pretty <see cref="string"/></returns>
        public static string PrettifyDictionary(IDictionary dictionary, PrettificationSettings settings = default) {
            const int    maxColWidth         = 50;
            const string headerSeparatorChar = "-";

            var keys = dictionary.Keys.Cast<object>().ToList();
            var vals = dictionary.Values.Cast<object>().ToList();

            // get the types of the columns, to use as headers
            var keyType = keys.First(it => it != null).GetType();
            var valType = vals.First(it => it != null).GetType();

            // add the headers
            keys.Insert(0, keyType);
            vals.Insert(0, valType);

            var keyLines = keys.Select(it => it.Prettify(PrettificationSettings.LineStyle.Single)).ToList();
            var valLines = vals.Select(it => it.Prettify(PrettificationSettings.LineStyle.Single)).ToList();

            var keyWidth = Math.Min(keyLines.LongestLine(), maxColWidth);
            var valWidth = Math.Min(valLines.LongestLine(), maxColWidth);

            // add the separators
            keyLines.Insert(1, headerSeparatorChar.Repeat(keyWidth));
            valLines.Insert(1, headerSeparatorChar.Repeat(valWidth));

            //NOTE: There is actually a version of select that uses the index! O_O
            var lines = keyLines.Select<string, string>((it, i) => PrettifyPair(it, valLines[i], keyWidth, valWidth)).ToList();
            return lines.JoinLines();
        }

        public static string PrettifyDictionary(IDictionary dictionary, Type keyType, Type valType) {
            const int    maxColWidth         = 50;
            const string headerSeparatorChar = "-";

            var keys = dictionary.Keys.Cast<object>().ToList();
            var vals = dictionary.Values.Cast<object>().ToList();

            // add the headers
            keys.Insert(0, keyType);
            vals.Insert(0, valType);

            var keyLines = keys.Select(it => it.Prettify(PrettificationSettings.LineStyle.Single)).ToList();
            var valLines = vals.Select(it => it.Prettify(PrettificationSettings.LineStyle.Single)).ToList();

            var keyWidth = Math.Min(keyLines.LongestLine(), maxColWidth);
            var valWidth = Math.Min(valLines.LongestLine(), maxColWidth);

            // add the separators
            keyLines.Insert(1, headerSeparatorChar.Repeat(keyWidth));
            valLines.Insert(1, headerSeparatorChar.Repeat(valWidth));

            //NOTE: There is actually a version of select that uses the index! O_O
            var lines = keyLines.Select<string, string>((it, i) => PrettifyPair(it, valLines[i], keyWidth, valWidth)).ToList();
            return lines.JoinLines();
        }

        private static string PrettifyPair(object a, object b, int aWidth, int bWidth, string separator = " ") {
            return $"{PrettifyCell(a, aWidth)}{separator}{PrettifyCell(b, bWidth)}";
        }

        private static string PrettifyCell(object cellValue, int columnWidth) {
            var str = cellValue.Prettify(PrettificationSettings.LineStyle.Single);
            return str.ForceToLength(columnWidth);
        }

        /**
         * <remarks>I would have this operate on <see cref="KeyedCollection{TKey,TItem}"/>, but unfortunately, <see cref="KeyedCollection{TKey,TItem}.GetKeyForItem"/> is <c>protected</c>.</remarks>
         */
        internal static string PrettifyKeyedList<TKey, TValue>(KeyedList<TKey, TValue> keyedList, PrettificationSettings settings = default) {
            // return PrettifyDictionary(keyedList.ToDictionary(), settings);
            return InnerPretty.PrettifyDictionary(keyedList.ToDictionary(), typeof(TKey), typeof(TValue));
        }
    }
}