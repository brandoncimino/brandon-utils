using System.ComponentModel;

using UnityEngine;

namespace Packages.BrandonUtils.Runtime {
    public static class RichTextUtils {
        #region FontStyle

        public static string Begin(this FontStyle fontStyle) {
            switch (fontStyle) {
                case FontStyle.Normal:
                    return "";
                case FontStyle.Bold:
                    return "<b>";
                case FontStyle.Italic:
                    return "<i>";
                case FontStyle.BoldAndItalic:
                    return "<b><i>";
                default:
                    throw new InvalidEnumArgumentException(nameof(fontStyle), (int) fontStyle, fontStyle.GetType());
            }
        }

        public static string End(this FontStyle fontStyle) {
            switch (fontStyle) {
                case FontStyle.Normal:
                    return "";
                case FontStyle.Bold:
                    return "</b>";
                case FontStyle.Italic:
                    return "</i>";
                case FontStyle.BoldAndItalic:
                    return "</i></b>";
                default:
                    throw new InvalidEnumArgumentException(nameof(fontStyle), (int) fontStyle, fontStyle.GetType());
            }
        }

        /// <summary>
        /// Formats <paramref name="toStylize"/> according to <paramref name="fontStyle"/>.
        /// </summary>
        /// <param name="fontStyle"></param>
        /// <param name="toStylize"></param>
        /// <returns></returns>
        /// <seealso cref="StringUtils.Stylize"/>
        /// <a href="https://docs.unity3d.com/2018.3/Documentation/Manual/StyledText.html">Unity Manual - Rich Text</a>
        public static string Stylize(this FontStyle fontStyle, string toStylize) {
            return $"{fontStyle.Begin()}{toStylize}{fontStyle.End()}";
        }

        #endregion

        #region Colors

        public static string Begin(this Color color) {
            var htmlString = ColorUtility.ToHtmlStringRGB(color);
            return $"<color=#{htmlString}>";
        }

        public static string End(this Color color) {
            return "</color>";
        }

        public static string Colorize(this Color color, string thing) {
            return $"{color.Begin()}{thing}{color.End()}";
        }

        #endregion
    }
}