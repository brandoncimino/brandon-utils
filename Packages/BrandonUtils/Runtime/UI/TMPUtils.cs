using System;
using System.Collections.Generic;
using System.Linq;

using BrandonUtils.Standalone.Collections;

using JetBrains.Annotations;

using TMPro;

// ReSharper disable MemberCanBePrivate.Global

namespace BrandonUtils.UI {
    /// <summary>
    /// Contains extensions and utilities for <see cref="TextMeshPro"/>.
    /// </summary>
    public static class TMPUtils {
        #region Parsing "words"

        /// <summary>
        /// Returns a <see cref="TMP_Word"/> corresponding to the <b>first</b> instance of <paramref name="substring"/>.
        /// </summary>
        /// <param name="text">The <see cref="TMP_Text"/> to parse.</param>
        /// <param name="substring">The substring to be found via <see cref="string.IndexOf(string, StringComparison)"/>.</param>
        /// <param name="stringComparisonMode">The <see cref="StringComparison"/> used by <see cref="string.IndexOf(string, StringComparison)"/>.<br/>Defaults to <see cref="StringComparison.Ordinal"/>.</param>
        /// <returns>The <see cref="TMP_Word"/> corresponding to the first instance of <paramref name="substring"/>.</returns>
        /// <seealso cref="LastSubstring"/>
        public static TMP_Word FirstSubstring(this TMP_Text text, string substring, StringComparison stringComparisonMode = StringComparison.Ordinal) {
            var foundIndex = text.text.IndexOf(substring, stringComparisonMode);
            if (foundIndex < 0) {
                return null;
            }
            else {
                return new TMP_Word(
                    text,
                    text.textInfo.VisibleCharacterInfo()
                        .ToList()
                        .GetRange(
                            foundIndex,
                            substring.Length
                        )
                        .ToList()
                );
            }
        }

        /// <summary>
        /// Returns a <see cref="TMP_Word"/> corresponding to the <b>last</b> instance of <paramref name="substring"/>.
        /// </summary>
        /// <param name="text">The <see cref="TMP_Text"/> to parse.</param>
        /// <param name="substring">The substring to be found via <see cref="string.LastIndexOf(string, StringComparison)"/>.</param>
        /// <param name="stringComparisonMode">The <see cref="StringComparison"/> used by <see cref="string.LastIndexOf(string, StringComparison)"/>.<br/>Defaults to <see cref="StringComparison.Ordinal"/>.</param>
        /// <returns>The <see cref="TMP_Word"/> corresponding to the last instance of <paramref name="substring"/>.</returns>
        /// <seealso cref="FirstSubstring"/>
        [Pure]
        public static TMP_Word LastSubstring(this TMP_Text text, string substring, StringComparison stringComparisonMode = StringComparison.Ordinal) {
            var lastIndexOf = text.text.LastIndexOf(substring, stringComparisonMode);

            if (lastIndexOf < 0) {
                return null;
            }

            var characters = text.textInfo.characterInfo.ToList()
                                 .GetRange(
                                     lastIndexOf,
                                     substring.Length
                                 )
                                 .ToList();

            return new TMP_Word(text, characters);
        }

        /// <summary>
        /// Builds a new <see cref="TMP_Word"/>.
        /// </summary>
        /// <param name="characterInfo"></param>
        /// <param name="textMesh"></param>
        /// <returns></returns>
        public static TMP_Word ToWord(this IEnumerable<TMP_CharacterInfo> characterInfo, TextMeshPro textMesh) {
            return new TMP_Word(textMesh, characterInfo);
        }

        #endregion

        #region Other

        public static string VisibleText(this TMP_Text text) {
            return text.textInfo.VisibleCharacterInfo().Select(ci => ci.character).JoinString();
        }

        public static IEnumerable<TMP_CharacterInfo> VisibleCharacterInfo(this TMP_TextInfo textInfo) {
            return textInfo.characterInfo.ToList().GetRange(0, textInfo.characterCount);
        }

        #endregion
    }
}