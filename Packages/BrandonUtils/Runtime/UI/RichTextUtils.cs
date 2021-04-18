using System.ComponentModel;

using UnityEngine;

namespace BrandonUtils.Strings {
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
        /// <seealso cref="Stylize(string,UnityEngine.FontStyle)"/>
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

        /// <summary>
        /// Formats <paramref name="toStylize"/> according to <paramref name="style"/>.
        /// </summary>
        /// <example>
        /// "yolo".<see cref="Stylize(string,UnityEngine.FontStyle)">Stylize</see>(<see cref="FontStyle.Bold"/>) == <![CDATA["<b>yolo</b>"]]>;
        /// </example>
        /// <param name="toStylize"></param>
        /// <param name="style"></param>
        /// <returns></returns>
        /// <seealso cref="Stylize(UnityEngine.FontStyle,string)"/>
        /// <a href="https://docs.unity3d.com/2018.3/Documentation/Manual/StyledText.html">Unity Manual - Rich Text</a>
        public static string Stylize(this string toStylize, FontStyle style) {
            return style.Stylize(toStylize);
        }

        /// <summary>
        /// Formats <paramref name="toColorize"/> according to <paramref name="color"/>.
        /// <p/>
        /// Is a no-op if <paramref name="color"/> is <c>null</c>.
        /// </summary>
        /// <param name="toColorize"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static string Colorize(this string toColorize, Color? color) {
            return color == null ? $"{toColorize}" : color.Value.Colorize(toColorize);
        }
    }
}