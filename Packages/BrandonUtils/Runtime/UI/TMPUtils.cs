using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

using Packages.BrandonUtils.Runtime.Collections;
using Packages.BrandonUtils.Runtime.Enums;
using Packages.BrandonUtils.Runtime.Logging;

using TMPro;

using UnityEngine;

using static UnityEngine.RectTransform.Edge;

// ReSharper disable MemberCanBePrivate.Global

namespace Packages.BrandonUtils.Runtime.UI {
    /// <summary>
    /// Contains extensions and utilities for <see cref="TextMeshPro"/>.
    /// </summary>
    /// <remarks>
    /// Many of these methods refer to "words", which in this context, are <see cref="ICollection{T}"/>s of <see cref="TMP_CharacterInfo"/>s.
    /// <ul>
    ///     For example:
    ///     <li><see cref="WordHeight"/></li>
    ///     <li><see cref="WordSizeDelta"/></li>
    /// </ul>
    /// "words" are usually returned <i>by</i> <see cref="TMPUtils"/> as <see cref="List{T}"/>s (see <see cref="FirstSubstring"/>)
    /// </remarks>
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
            var foundIndex = text.text.IndexOf(substring, stringComparisonMode);
            if (foundIndex < 0) {
                return null;
            }
            else {
                return text.textInfo.VisibleCharacterInfo()
                           .ToList()
                           .GetRange(
                               foundIndex,
                               substring.Length
                           )
                           .ToList();
            }
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
            var lastIndexOf = text.text.LastIndexOf(substring, stringComparisonMode);

            if (lastIndexOf < 0) {
                return null;
            }

            LogUtils.Log(
                $"Content: {text.text}",
                $"Length: {text.text.Length}",
                $"C.Info: {text.textInfo.characterInfo.JoinString()}",
                $"Chars: {text.textInfo.characterInfo.Select(ci => ci.character).JoinString()}"
            );

            Debug.Log($"{nameof(lastIndexOf)}: {lastIndexOf}");
            return text.textInfo.characterInfo.ToList()
                       .GetRange(
                           lastIndexOf,
                           substring.Length
                       )
                       .ToList();
        }

        #endregion

        #region Measuring "word" dimensions

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

        public static float WordDimension(this ICollection<TMP_CharacterInfo> word, RectTransform.Axis dimension) {
            switch (dimension) {
                case RectTransform.Axis.Horizontal:
                    return word.WordWidth();
                case RectTransform.Axis.Vertical:
                    return word.WordHeight();
                default:
                    throw EnumUtils.InvalidEnumArgumentException(nameof(dimension), dimension);
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

        #region Retrieving "word" TextAnchor positions

        /// <summary>
        /// Returns the <see cref="WordAnchorPosition"/> converted to a <b>proportion</b> (<see cref="Mathf.Clamp01"/>ed) of <paramref name="parent"/>'s <see cref="RectTransform.sizeDelta"/>, for use as an <see cref="RectTransform.anchorMin"/> or <see cref="RectTransform.anchorMax"/>.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="word"></param>
        /// <param name="textAnchor"></param>
        /// <returns></returns>
        public static Vector2 WordAnchor(RectTransform parent, ICollection<TMP_CharacterInfo> word, TextAnchor textAnchor) {
            var anchorPos = word.WordAnchorPosition(textAnchor);
            var sizeDelta = parent.sizeDelta;
            return new Vector2(
                anchorPos.x / sizeDelta.x,
                anchorPos.y / sizeDelta.y
            );
        }

        /// <summary>
        /// Returns the local position of <paramref name="word"/>'s <paramref name="textAnchor"/>, e.g. <see cref="TextAnchor.MiddleLeft"/>.
        /// </summary>
        /// <param name="word"></param>
        /// <param name="textAnchor"></param>
        /// <returns></returns>
        public static Vector2 WordAnchorPosition(this ICollection<TMP_CharacterInfo> word, TextAnchor textAnchor) {
            var leftPos = word.WordEdge(Left);
            var width   = word.WordWidth();
            var x       = leftPos + (textAnchor.Anchor().x * width);

            var bottomPos = word.WordEdge(Bottom);
            var height    = word.WordHeight();
            var y         = bottomPos + (textAnchor.Anchor().y * height);

            return new Vector2(x, y);
        }

        #endregion

        #region Manipulating "words"

        public static RectTransform HighlightWord(ICollection<CharacterInfo> word, RectTransform highlight) {
            // highlight.anchormi
            throw new NotImplementedException();
        }

        #endregion

        #region Other

        public static string VisibleText(this TMP_Text text) {
            // return text.textInfo.characterInfo.Where(ch => ch.isVisible).Select(ch => ch.character).JoinString();
            // return text.textInfo.characterInfo.ToList().GetRange(0, text.textInfo.characterCount).Select(ch => ch.character).JoinString();
            // return text.textInfo.characterInfo.Select(ch => ch.character).JoinString();
            // return string.Join("",text.textInfo.characterInfo.Select(ch => ch.character));
            return text.textInfo.VisibleCharacterInfo().Select(ci => ci.character).JoinString();
        }

        public static IEnumerable<TMP_CharacterInfo> VisibleCharacterInfo(this TMP_TextInfo textInfo) {
            return textInfo.characterInfo.ToList().GetRange(0, textInfo.characterCount);
        }

        #endregion
    }
}