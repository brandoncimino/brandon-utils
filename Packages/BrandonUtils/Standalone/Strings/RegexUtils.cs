using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using BrandonUtils.Standalone.Collections;
using BrandonUtils.Standalone.Optional;

namespace BrandonUtils.Standalone.Strings {
    /// <summary>
    /// Mostly contains extension method version of basic <see cref="Regex"/> stuff.
    /// </summary>
    public static class RegexUtils {
        #region Match

        /// <returns><see cref="Regex.Match(string,string)"/></returns>
        public static Match Match(this string str, string pattern) {
            return Regex.Match(str, pattern);
        }

        /// <returns><see cref="Regex.Match(string, string, RegexOptions)"/></returns>
        public static Match Match(this string str, string pattern, RegexOptions options) {
            return Regex.Match(str, pattern, options);
        }

        /// <returns><see cref="Regex.Match(string)"/></returns>
        public static Match Match(this string str, Regex pattern) {
            return pattern.Match(str);
        }

        #endregion

        #region AllMatches

        /// <summary>
        /// This is an extension method for <see cref="Regex.Matches(string)"/>.</summary>
        /// <remarks>
        /// <inheritdoc cref="Matches(string,string)"/>
        /// </remarks>
        /// <returns><see cref="Regex.Matches(string, string)"/></returns>
        public static MatchCollection AllMatches(this string str, string pattern) {
            return Regex.Matches(str, pattern);
        }

        /// <inheritdoc cref="AllMatches(string,string)"/>
        /// <returns><see cref="Regex.Matches(string, string, RegexOptions)"/></returns>
        public static MatchCollection AllMatches(this string str, string pattern, RegexOptions options) {
            return Regex.Matches(str, pattern, options);
        }

        /// <inheritdoc cref="AllMatches(string,string)"/>
        /// <returns><see cref="Regex.Matches(string)"/></returns>
        public static MatchCollection AllMatches(this string str, Regex pattern) {
            return pattern.Matches(str);
        }

        #endregion

        #region Matches

        /// <summary>
        /// This is an extension method for <see cref="Regex.IsMatch(string)"/>.
        /// </summary>
        /// <remarks>
        /// I am aware that "Matches" would correspond to <see cref="Regex.Matches(string)"/> for consistency.
        /// However, that method name causes me intense physical discomfort.
        /// If you want to call that stupid dumbly named dumb bad method, use <see cref="AllMatches(string,string)"/> instead.
        /// <p/>
        /// Strangely, despite the hideous 80-thousand extra steps of using <a href="https://docs.oracle.com/javase/8/docs/api/java/util/regex/Pattern.html">Java's Regex library</a>, Java actually uses the correct verb, <a href="https://docs.oracle.com/javase/8/docs/api/java/util/regex/Pattern.html#matches-java.lang.String-java.lang.CharSequence-">Matches()</a>.
        /// </remarks>
        /// <returns><see cref="Regex.IsMatch(string, string)"/></returns>
        public static bool Matches(this string str, string pattern) {
            return Regex.IsMatch(str, pattern);
        }

        /// <summary>
        /// <inheritdoc cref="Matches(string,string)"/>
        /// </summary>
        /// <remarks>
        /// <inheritdoc cref="Matches(string,string)"/>
        /// </remarks>
        /// <param name="str"><inheritdoc cref="Matches(string,string)"/></param>
        /// <param name="pattern"><inheritdoc cref="Matches(string,string)"/></param>
        /// <param name="options">any applicable <see cref="RegexOptions"/>, like <see cref="RegexOptions.Multiline"/></param>
        /// <returns><see cref="Regex.IsMatch(string, string, RegexOptions)"/></returns>
        public static bool Matches(this string str, string pattern, RegexOptions options) {
            return Regex.IsMatch(str, pattern, options);
        }

        /// <inheritdoc cref="Matches(string,string)"/>
        /// <returns><see cref="Regex.IsMatch(string)"/></returns>
        public static bool Matches(this string str, Regex pattern) {
            return pattern.IsMatch(str);
        }

        #endregion

        #region MatchesAll

        public static bool MatchesAll(this string str, IEnumerable<string> patterns, RegexOptions? options = default) {
            return options.IfOrElse(
                rOpts => patterns.All(pat => Regex.IsMatch(str, pat, rOpts)),
                () => patterns.All(pat => Regex.IsMatch(str,    pat))
            );
        }

        public static bool MatchesAll(this string str, params string[] patterns) {
            return MatchesAll(str, patterns.AsEnumerable());
        }

        public static bool MatchesAll(this string str, params Regex[] patterns) {
            return MatchesAll(str, patterns.AsEnumerable());
        }

        public static bool MatchesAll(this string str, IEnumerable<Regex> patterns, RegexOptions? options = default) {
            return patterns.All(it => it.IsMatch(str));
        }

        #endregion

        #region MatchesAny

        public static bool MatchesAny(this string str, IEnumerable<string> patterns, RegexOptions? options = default) {
            return options.IfOrElse(
                rOpts => patterns.Any(pat => Regex.IsMatch(str, pat, rOpts)),
                () => patterns.Any(pat => Regex.IsMatch(str,    pat))
            );
        }

        public static bool MatchesAny(this string str, params string[] patterns) {
            return MatchesAny(str, patterns.AsEnumerable());
        }

        public static bool MatchesAny(this string str, IEnumerable<Regex> patterns) {
            return patterns.Any(it => it.IsMatch(str));
        }

        public static bool MatchesAny(this string str, params Regex[] patterns) {
            return MatchesAny(str, patterns.AsEnumerable());
        }

        #endregion

        #region MatchesNone

        public static bool MatchesNone(this string str, IEnumerable<string> patterns, RegexOptions? options = default) {
            return options.IfOrElse(
                rOpts => patterns.None(pat => Regex.IsMatch(str, pat, rOpts)),
                () => patterns.None(pat => Regex.IsMatch(str,    pat))
            );
        }

        public static bool MatchesNone(this string str, params string[] patterns) {
            return MatchesNone(str, patterns.AsEnumerable());
        }

        public static bool MatchesNone(this string str, IEnumerable<Regex> patterns) {
            return patterns.None(it => it.IsMatch(str));
        }

        public static bool MatchesNone(this string str, params Regex[] patterns) {
            return MatchesNone(str, patterns.AsEnumerable());
        }

        #endregion
    }
}