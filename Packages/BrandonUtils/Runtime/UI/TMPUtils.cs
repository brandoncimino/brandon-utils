using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

using Packages.BrandonUtils.Runtime.Enums;

using TMPro;

using UnityEngine;

using static UnityEngine.RectTransform.Edge;

// ReSharper disable MemberCanBePrivate.Global

namespace Packages.BrandonUtils.Runtime.UI {
    /// <summary>
    /// Contains extensions and utilities for <see cref="TextMeshPro"/>.
    /// </summary>
    public static class TMPUtils {
        #region Parsing "words"

        /// <summary>
        /// Returns a "word" (a collection of <see cref="TMP_CharacterInfo"/>) corresponding to the <b>first</b> instance of <paramref name="substring"/>.
        /// </summary>
        /// <param name="text">The <see cref="TMP_Text"/> to parse.</param>
        /// <param name="substring">The substring to be found via <see cref="string.IndexOf(string, StringComparison)"/>.</param>
        /// <param name="stringComparisonMode">The <see cref="StringComparison"/> used by <see cref="string.IndexOf(string, StringComparison)"/>.<br/>Defaults to <see cref="StringComparison.Ordinal"/>.</param>
        /// <returns>The <see cref="TMP_CharacterInfo"/>s corresponding to the first instance of <paramref name="substring"/>.</returns>
        /// <seealso cref="LastSubstring"/>
        public static List<TMP_CharacterInfo> FirstSubstring(this TMP_Text text, string substring, StringComparison stringComparisonMode = StringComparison.Ordinal) {
            return text.textInfo.characterInfo.ToList()
                       .GetRange(
                           text.text.IndexOf(
                               substring,
                               stringComparisonMode
                           ),
                           substring.Length
                       )
                       .ToList();
        }

        /// <summary>
        /// Returns a "word" (a collection of <see cref="TMP_CharacterInfo"/>) corresponding to the <b>last</b> instance of <paramref name="substring"/>.
        /// </summary>
        /// <param name="text">The <see cref="TMP_Text"/> to parse.</param>
        /// <param name="substring">The substring to be found via <see cref="string.LastIndexOf(string, StringComparison)"/>.</param>
        /// <param name="stringComparisonMode">The <see cref="StringComparison"/> used by <see cref="string.LastIndexOf(string, StringComparison)"/>.<br/>Defaults to <see cref="StringComparison.Ordinal"/>.</param>
        /// <returns>The <see cref="TMP_CharacterInfo"/>s corresponding to the last instance of <paramref name="substring"/>.</returns>
        /// <seealso cref="FirstSubstring"/>
        [Pure]
        public static List<TMP_CharacterInfo> LastSubstring(this TMP_Text text, string substring, StringComparison stringComparisonMode = StringComparison.Ordinal) {
            return text.textInfo.characterInfo.ToList()
                       .GetRange(
                           text.text.LastIndexOf(
                               substring,
                               stringComparisonMode
                           ),
                           substring.Length
                       )
                       .ToList();
        }

        #endregion

        #region Measuring "words" for UI RectTransform stuff

        [Pure]
        public static float WordEdge(this IEnumerable<TMP_CharacterInfo> word, RectTransform.Edge edge) {
            switch (edge) {
                case Left:
                    return word.First().bottomLeft.x;
                case Right:
                    return word.Last().bottomRight.x;
                case Top:
                    return word.Max(letter => letter.topLeft.y);
                case Bottom:
                    return word.Min(letter => letter.bottomLeft.y);
                default:
                    throw EnumUtils.InvalidEnumArgumentException(nameof(edge), edge);
            }
        }

        /// <summary>
        /// Returns the width of <paramref name="word"/>, measured as the distance between the <see cref="RectTransform.Edge.Right"/> and <see cref="RectTransform.Edge.Left"/> (via <see cref="WordEdge"/>)
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        [Pure]
        public static float WordWidth(this ICollection<TMP_CharacterInfo> word) {
            return word.WordEdge(Right) - word.WordEdge(Left);
        }

        /// <summary>
        /// Returns the height of <paramref name="word"/>, measured as the distance between the <see cref="RectTransform.Edge.Top"/> and <see cref="RectTransform.Edge.Bottom"/> (via <see cref="WordEdge"/>)
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        [Pure]
        public static float WordHeight(this ICollection<TMP_CharacterInfo> word) {
            return word.WordEdge(Top) - word.WordEdge(Bottom);
        }

        /// <summary>
        /// Combines <see cref="WordWidth"/> and <see cref="WordHeight"/> into a <see cref="Vector2"/> for use as a <see cref="RectTransform.sizeDelta"/>.
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static Vector2 WordSizeDelta(this ICollection<TMP_CharacterInfo> word) {
            return new Vector2(word.WordWidth(), word.WordHeight());
        }

        #endregion
    }
}