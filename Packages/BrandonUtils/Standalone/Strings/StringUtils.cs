using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

using BrandonUtils.Standalone.Collections;
using BrandonUtils.Standalone.Enums;

using FowlFever.Conjugal.Affixing;

using JetBrains.Annotations;

using Newtonsoft.Json;

using Pure = System.Diagnostics.Contracts.PureAttribute;

namespace BrandonUtils.Standalone.Strings {
    [PublicAPI]
    public static class StringUtils {
        public const int DefaultIndentSize = 2;

        /// <summary>
        /// A <see cref="string"/> for a single-glyph <a href="https://en.wikipedia.org/wiki/Ellipsis">ellipsis</a>, i.e. <c>'…'</c>.
        ///
        /// <ul>
        /// <li><b>Unicode:</b> <c>U+2026 … HORIZONTAL ELLIPSIS</c></li>
        /// <li><b>HTML:</b> <c><![CDATA[&#8230;]]></c></li>
        /// </ul>
        /// </summary>
        internal const string Ellipsis = "…";

        /// <summary>
        /// A <see cref="string"/> for the glyph representing the <a href="https://en.wikipedia.org/wiki/Tab_key#Unicode">"tab" key</a>, i.e. one indent.
        ///
        /// <ul>
        /// <li><b>Unicode:</b> <c>U+21E5 ⇥ RIGHTWARDS ARROW TO BAR</c></li>
        /// </ul>
        /// </summary>
        internal const string TabArrow = "⇥";

        internal const char DefaultIndentChar = ' ';

        /// <summary>
        /// Valid strings for <a href="https://en.wikipedia.org/wiki/Newline">line breaks</a>, in <b>order of precedence</b> as required by <see cref="string.Split(string[], StringSplitOptions)"/>:
        /// <ul>
        /// <li><c>"\r\n"</c>, aka "Carriage Return + Line Feed", aka <c>"CRLF"</c></li>
        /// <li><c>"\r"</c>, aka <a href="https://en.wikipedia.org/wiki/Carriage_return">"Carriage Return"</a>, aka <c>"CR"</c></li>
        /// <li><c>"\n"</c>, aka <a href="https://en.wikipedia.org/wiki/Newline#In_programming_languages">"Newline"</a>, aka "Line Feed", aka <c>"LF"</c></li>
        /// </ul>
        /// </summary>
        /// <remarks>
        /// Intended to passed to <see cref="string.Split(string[], StringSplitOptions)"/>.
        /// </remarks>
        internal static readonly string[] LineBreakSplitters = { "\r\n", "\r", "\n" };

        /// <summary>
        /// Prepends <paramref name="toIndent"/>.
        /// </summary>
        /// <param name="toIndent">The <see cref="string" /> to be indented.</param>
        /// <param name="indentCount">The number of indentations (i.e. number of times hitting "tab").</param>
        /// <param name="indentSize">The <see cref="string.Length"/> of each individual indentation. Defaults to <see cref="DefaultIndentSize"/>.</param>
        /// <param name="indentChar">The <see cref="char"/> that is <see cref="Repeat(char,int,string)"/>ed to build a single indentation. Defaults to <see cref="DefaultIndentChar"/>.</param>
        /// <returns>The indented <see cref="string"/>.</returns>
        /// <seealso cref="Indent(IEnumerable{string},int,int,char)"/>
        public static string Indent(
            [CanBeNull] this string toIndent,
            [NonNegativeValue]
            int indentCount = 1,
            [NonNegativeValue]
            int indentSize = DefaultIndentSize,
            char indentChar = ' '
        ) {
            if (indentCount.IsPositive() == false) {
                throw new ArgumentOutOfRangeException(nameof(indentCount));
            }

            if (indentSize.IsPositive() == false) {
                throw new ArgumentOutOfRangeException(nameof(indentSize));
            }

            return indentChar
                   .Repeat(indentSize)
                   .Repeat(indentCount)
                   .Suffix(toIndent);
        }

        /// <summary>
        /// Joins <paramref name="toRepeat" /> with itself <paramref name="repetitions" /> times, using the optional <paramref name="separator" />
        /// </summary>
        /// <param name="toRepeat">The <see cref="string" /> to be joined with itself.</param>
        /// <param name="repetitions">The number of times <paramref name="toRepeat" /> should be repeated.</param>
        /// <param name="separator">An optional character, analogous to </param>
        /// <returns></returns>
        [ContractAnnotation("toRepeat:null => null")]
        [ContractAnnotation("toRepeat:notnull => notnull")]
        [CanBeNull]
        public static string Repeat([CanBeNull] this string toRepeat, [NonNegativeValue] int repetitions, [CanBeNull] string separator = "") {
            if (repetitions.IsPositive() == false) {
                throw new ArgumentOutOfRangeException(nameof(repetitions));
            }

            if (toRepeat == null) {
                return null;
            }

            var list = new List<string>();
            for (var i = 0; i < repetitions; i++) {
                list.Add(toRepeat);
            }

            return string.Join(separator, list);
        }

        /// <inheritdoc cref="Repeat(string,int,string)" />
        [NotNull]
        public static string Repeat(this char toRepeat, [NonNegativeValue] int repetitions, string separator = "") {
            if (repetitions.IsPositive() == false) {
                throw new ArgumentOutOfRangeException(nameof(repetitions));
            }

            return Repeat(toRepeat.ToString(), repetitions, separator);
        }

        /// <summary>
        ///     Joins together <paramref name="baseString" /> and <paramref name="stringToJoin" /> via <paramref name="separator" />,
        ///     <b>
        ///         <i>UNLESS</i>
        ///     </b>
        ///     <paramref name="baseString" /> is <c>null</c>, in which case <paramref name="stringToJoin" /> is returned.
        /// </summary>
        /// <remarks>
        ///     The idea of this is that it can be used to build out a single string and "list out" items, rather than building a <see cref="List{T}" /> and calling <see cref="string.Join(string,System.Collections.Generic.IEnumerable{string})" /> against it.
        /// </remarks>
        /// <example>
        ///     <code><![CDATA[
        /// "yolo".Join("swag")     -> yoloswag
        /// ]]></code>
        ///     <code><![CDATA[
        /// "yolo".Join("swag",":") -> yolo; swag
        /// ]]></code>
        ///     <code><![CDATA[
        /// "".Join("swag", ":")    -> swag
        /// ]]></code>
        ///     <code><![CDATA[
        /// null.Join(":")          -> swag
        /// ]]></code>
        /// </example>
        /// <param name="baseString"></param>
        /// <param name="stringToJoin"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string Join(this string baseString, string stringToJoin, string separator = "") {
            return string.IsNullOrEmpty(baseString) ? stringToJoin : string.Join(separator, baseString, stringToJoin);
        }

        #region Prettification

        [Obsolete]
        public static string Prettify(object thing, bool recursive = true, int recursionCount = 0) {
            const int recursionMax = 10;
            var       type         = thing.GetType().ToString();
            string    method       = null;
            string    prettyString = null;

            switch (thing) {
                //don't do anything special with strings
                //check for value types (int, char, etc.), which we shouldn't do anything fancy with
                //TODO 7/4/2021: The line written above is a BOLD FACED LIE
                //  Actually, this whole method is just hot garbage and needs to be REBORN
                case string s:
                    method       = "string";
                    prettyString = s;
                    break;
                case ValueType _:
                    method       = nameof(ValueType);
                    prettyString = thing.ToString();
                    break;
                case IEnumerable enumerableThing:
                    method = $"recursion, {recursionCount}";

                    recursionCount++;

                    if (!recursive || recursionCount >= recursionMax) {
                        goto default;
                    }

                    foreach (var entry in enumerableThing) {
                        prettyString += "\n" + Prettify(entry, true, recursionCount);
                    }

                    break;
                default:
                    try {
                        method       = "JSON";
                        prettyString = JsonConvert.SerializeObject(thing, Formatting.Indented);
                    }
                    catch (Exception) {
                        method = "JSON - FAILED!";
                    }

                    break;
            }

            // account for null prettyString and method
            // (we're doing this here, rather than initializing them to default values, so we can trigger things if there's a failure)
            // NOTE from Brandon on 8/22/2021: this fancy-schmancy ??= operator is...funky
            prettyString ??= thing.ToString();
            method       ??= "NO METHOD FOUND";


            return $"[{method}]{prettyString}".Indent(indentCount: recursionCount);
        }

        //TODO: Add an extension method version of "Prettify" and re-do the fuck out of "Prettify"
        //TODO: Move prettification methods into a dedicated class
        //TODO: the fuck is the difference between "Prettify" and "Pretty"?! "Prettify" is definitely a better name!
        public static string Pretty(this object obj) {
            throw new NotImplementedException("DEAR GOD I immediately started making a 'PrettyOptions' enum to go along with this ARGH");
        }

        public static string ListVariables(object obj) {
            return ListMembers(obj, MemberTypes.Property | MemberTypes.Field);
        }

        public static string ListProperties(object obj) {
            return ListMembers(obj, MemberTypes.Property);
        }

        public static string ListFields(object obj) {
            return ListMembers(obj, MemberTypes.Field);
        }

        public static string ListMembers(object obj, MemberTypes memberTypes = MemberTypes.All) {
            //if obj is a already a type, cast it and use it; otherwise, grab its type
            Type objType = obj is Type type ? type : obj.GetType();
            return objType.GetMembers().Where(member => memberTypes.HasFlag(member.MemberType)).Aggregate($"[{objType}] {memberTypes}:", (current, member) => current + $"\n\t{FormatMember(member, obj)}");
        }

        /// <summary>
        /// TODO: rename this to "PrettifyMember"
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string FormatMember(MemberInfo memberInfo, object obj = null) {
            var result = $"[{memberInfo.MemberType}] {memberInfo}";

            try {
                if (obj != null) {
                    switch (memberInfo) {
                        case PropertyInfo propertyInfo:
                            result += $": {propertyInfo.GetValue(obj)}";
                            break;
                        case FieldInfo fieldInfo:
                            result += $": {fieldInfo.GetValue(obj)}";
                            break;
                    }
                }
            }
            catch (Exception e) {
                result += $"ERROR: {e.Message}";
            }

            return result;
        }

        #endregion

        #region Padding, filling, truncating, trimming, and trailing

        /// <summary>
        /// Reduces <paramref name="self"/> to <paramref name="maxLength"/> characters, replacing the last bits with a <paramref name="trail"/> if specified.
        ///
        /// If the original <see cref="string.Length"/> is less than <paramref name="maxLength"/>, returns <paramref name="self"/>.
        /// </summary>
        /// <param name="self">the <see cref="string"/> being truncated</param>
        /// <param name="maxLength">the <b>maximum</b> size of the final string</param>
        /// <param name="trail">a <see cref="string"/> to replace the end bits of <paramref name="self"/> to show that it has been truncated. Defaults to an <see cref="Ellipsis"/></param>
        /// <returns></returns>
        public static string Truncate([CanBeNull] this string self, [ValueRange(0, long.MaxValue)] int maxLength, [CanBeNull] string trail = Ellipsis) {
            if (self == null) {
                return "";
            }

            if (self.Length > maxLength) {
                var shortened = self.Substring(0, maxLength - trail?.Length ?? 0);
                return $"{shortened}{trail}";
            }

            return self;
        }

        /// <summary>
        /// Uses either <see cref="Truncate"/> or <see cref="FillRight"/> to get <paramref name="self"/> to be <paramref name="desiredLength"/> long.
        /// </summary>
        /// <param name="self">the original <see cref="string"/></param>
        /// <param name="desiredLength">the <see cref="string.Length"/> that <paramref name="self"/> will have</param>
        /// <param name="filler">the <see cref="string"/> used to <see cref="FillRight"/> if <paramref name="self"/> is shorter than <paramref name="desiredLength"/></param>
        /// <param name="trail">the <see cref="string"/> used to indicated that <paramref name="self"/> has been <see cref="Truncate"/>d</param>
        /// <returns>a <see cref="string"/> with a <see cref="string.Length"/> of <paramref name="desiredLength"/></returns>
        /// <exception cref="ArgumentException"></exception>
        [NotNull]
        public static string ForceToLength([CanBeNull] this string self, [ValueRange(0, long.MaxValue)] int desiredLength, [CanBeNull] string filler = " ", [CanBeNull] string trail = Ellipsis) {
            self ??= "";

            return self.Length.CompareTo(desiredLength).Sign() switch {
                -1 => self.FillRight(desiredLength, filler),
                0  => self,
                1  => self.Truncate(desiredLength, trail),
                _  => throw new ArgumentException($"This should be unreachable, because we used a {nameof(int.CompareTo)} function AND got the {nameof(Mathb.Sign)} of it, so we definitely should've only had the possibilities of -1, 0, or 1.")
            };
        }

        public static string FillRight(this string self, int totalLength, string filler) {
            ValidateFillParameters(filler, totalLength);

            if (self.Length >= totalLength) {
                return self;
            }

            var additionalLengthNeeded = totalLength - self.Length;
            return self + filler.Fill(additionalLengthNeeded);
        }

        public static string FillLeft(this string self, int totalLength, string filler) {
            ValidateFillParameters(filler, totalLength);

            if (self.Length >= totalLength) {
                return self;
            }

            var additionalLengthNeeded = totalLength - self.Length;
            return self + filler.Fill(additionalLengthNeeded).Reverse().JoinString();
        }

        public static string Fill(this string filler, int totalLength) {
            ValidateFillParameters(filler, totalLength);

            var fullLength  = totalLength / filler.Length;
            var extraLength = totalLength % filler.Length;
            var filled      = filler.Repeat(fullLength) + filler.Substring(0, extraLength);
            return filled;
        }

        private static void ValidateFillParameters(string filler, int totalLength) {
            if (filler == null) {
                throw new ArgumentNullException(nameof(filler));
            }

            if (string.IsNullOrEmpty(filler)) {
                throw new ArgumentException($"Cannot fill with an empty string!", nameof(filler));
            }

            if (totalLength < 0) {
                throw new ArgumentOutOfRangeException(nameof(totalLength), "Must be positive");
            }
        }

        public static string FormatHeading(string heading, string border = "=", string padding = " ") {
            var middle = $"{border}{padding}{heading}{padding}{border}";
            var hRule  = border.FillRight(middle.Length, border);
            return $"{hRule}\n{middle}\n{hRule}";
        }

        #endregion

        /// <summary>
        /// An extension method to call <see cref="Regex.Split(string)"/>.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static string[] Splitex(this string input, string pattern) {
            return Regex.Split(input, pattern);
        }

        /**
         * <inheritdoc cref="Splitex(string,string)"/>
         */
        public static string[] Splitex(this string input, string pattern, RegexOptions options) {
            return Regex.Split(input, pattern, options);
        }

        /**
         * <inheritdoc cref="Splitex(string,string)"/>
         */
        public static string[] Splitex(this string input, string pattern, RegexOptions options, TimeSpan matchTimeout) {
            return Regex.Split(input, pattern, options, matchTimeout);
        }

        /// <summary>
        /// Returns true if the given <see cref="string"/> <see cref="string.Contains"/> <b>any</b> of the given <see cref="substrings"/>.
        /// </summary>
        /// <remarks>
        /// TODO: I wonder if it would be faster to search for the shortest substring first...? And maybe excluding substrings that wholly contain other substrings?
        /// </remarks>
        /// <param name="str">the <see cref="string"/> to check for <see cref="substrings"/></param>
        /// <param name="substrings">the possible <see cref="substrings"/></param>
        /// <returns>true if the given <see cref="string"/> <see cref="string.Contains"/> <b>any</b> of the given <see cref="substrings"/></returns>
        public static bool ContainsAny(this string str, IEnumerable<string> substrings) {
            return substrings.Any(str.Contains);
        }

        /**
         * <inheritdoc cref="ContainsAny(string,System.Collections.Generic.IEnumerable{string})"/>
         */
        public static bool ContainsAny(this string str, params string[] substrings) {
            return ContainsAny(str, substrings.AsEnumerable());
        }

        /// <summary>
        /// Returns true if the given <see cref="string"/> <see cref="string.Contains"/> <b>all</b> of the given <see cref="substrings"/>.
        /// </summary>
        /// <param name="str">the <see cref="string"/> to search through</param>
        /// <param name="substrings">the possible substrings</param>
        /// <returns>true if the given <see cref="string"/> <see cref="string.Contains"/> <b>all</b> of the given <see cref="substrings"/></returns>
        public static bool ContainsAll(this string str, IEnumerable<string> substrings) {
            return substrings.All(str.Contains);
        }

        /**
         * <inheritdoc cref="ContainsAll(string,System.Collections.Generic.IEnumerable{string})"/>
         */
        public static bool ContainsAll(this string str, params string[] substrings) {
            return ContainsAll(str, substrings.AsEnumerable());
        }

        #region Line Management

        /// <summary>
        /// Splits <paramref name="contentWithLines"/> via <c>"\r\n", "\r", or "\n"</c>.
        /// </summary>
        /// <example>
        /// This will match any
        /// </example>
        /// <param name="contentWithLines">the <see cref="string"/> being <see cref="string.Split(char[])"/></param>
        /// <param name="options"><see cref="StringSplitOptions"/></param>
        /// <returns>an <see cref="Array"/> containing each individual line from <paramref name="contentWithLines"/></returns>
        public static string[] SplitLines(this string contentWithLines, StringSplitOptions options = default) {
            return contentWithLines.Split(LineBreakSplitters, options);
        }

        /// <summary>
        /// Runs <see cref="SplitLines(string,System.StringSplitOptions)"/> against each <see cref="string"/> in <paramref name="contentsWithLines"/>,
        /// flattening the results.
        /// </summary>
        /// <param name="contentsWithLines">a collection of <see cref="string"/>s that will each be passed to <see cref="SplitLines(string,System.StringSplitOptions)"/></param>
        /// <param name="options"><see cref="StringSplitOptions"/></param>
        /// <returns>all of the individual <see cref="string"/>s, split line-by-line, and flattened</returns>
        /// <seealso cref="SplitLines(string,System.StringSplitOptions)"/>
        /// <seealso cref="ToStringLines"/>
        public static string[] SplitLines(this IEnumerable<string> contentsWithLines, StringSplitOptions options = default) {
            return contentsWithLines.SelectMany(content => content.SplitLines(options)).ToArray();
        }

        public static int LongestLine(this IEnumerable<string> strings) {
            return strings.SelectMany(it => it.SplitLines()).Max(it => it.Length);
        }

        public static int LongestLine(this string str) {
            return str.SplitLines().Max(it => it.Length);
        }

        /// <summary>
        /// <see cref="Indent(string,int,int,char)"/>s each <see cref="string"/> in <paramref name="lines"/>.
        /// </summary>
        /// <param name="lines">a collection of <see cref="string"/>s which are treated as separate lines</param>
        /// <param name="indentCount">how many "indents" to add, i.e. how many times the "tab" key should be hit</param>
        /// <param name="indentSize">the <see cref="string.Length"/> of a single indent. Defaults to <see cref="DefaultIndentSize"/></param>
        /// <param name="indentChar">the <see cref="char"/> that is <see cref="Repeat(char,int,string)"/>ed to form a single indent. Defaults to <see cref="DefaultIndentChar"/></param>
        /// <returns>the indented <see cref="string"/>s</returns>
        /// <seealso cref="Indent(string,int,int,char)"/>
        [ContractAnnotation("lines:null => null")]
        [CanBeNull]
        public static IEnumerable<string> Indent(
            [CanBeNull] [ItemCanBeNull]
            this IEnumerable<string> lines,
            [NonNegativeValue]
            int indentCount = 1,
            [NonNegativeValue]
            int indentSize = DefaultIndentSize,
            char indentChar = DefaultIndentChar
        ) {
            if (indentCount.IsPositive() == false) {
                throw new ArgumentOutOfRangeException(nameof(indentCount));
            }

            if (indentSize.IsPositive() == false) {
                throw new ArgumentOutOfRangeException(nameof(indentSize));
            }

            return lines?.Select(it => it.Indent(indentCount, indentSize, indentChar)).ToList();
        }

        [ContractAnnotation("lines:null => null")]
        [CanBeNull]
        public static IEnumerable<string> IndentWithLabel([CanBeNull] this IEnumerable<string> lines, [CanBeNull] string label, [CanBeNull] string joiner = " ") {
            if (lines == null) {
                return null;
            }

            var firstLinePrefix = $"{label}{joiner}";
            var otherLinePrefix = $" ".Repeat(firstLinePrefix.Length);
            return lines.Select(
                (line, i) => i == 0 ? $"{firstLinePrefix}{line}" : $"{otherLinePrefix}{line}"
            );
        }

        #region Truncation & Collapsing

        /// <summary>
        /// Returns the first <see cref="lineCount"/> full lines of <see cref="lines"/>.
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="lineCount"></param>
        /// <param name="includeMessage"></param>
        /// <returns></returns>
        public static string[] TruncateLines(this IEnumerable<string> lines, int lineCount, bool includeMessage = true) {
            var lns = lines.ToArray();
            if (lns.Length <= lineCount) {
                return lns.ToArray();
            }

            if (includeMessage) {
                return lns.Take(lineCount - 1)
                          .Append($"{Ellipsis}[{lns.Length - lineCount} lines omitted]")
                          .ToArray();
            }
            else {
                return lns.Take(lineCount)
                          .ToArray();
            }
        }

        public static string[] TruncateLines(this string contentWithLines, int lineCount, bool includeMessage = true) {
            return TruncateLines(contentWithLines.SplitLines(), lineCount, includeMessage);
        }

        public static string[] CollapseLines(string[] lines, Func<string, bool> predicate) {
            var  filteredLines = new List<string>();
            int? collapseFrom  = null;
            for (int i = 0; i < lines.Length; i++) {
                var matches = predicate.Invoke(lines[i]);

                // NOT currently collapsing
                if (!collapseFrom.HasValue) {
                    // Starting to collapse
                    if (matches) {
                        collapseFrom = i;
                        continue;
                    }

                    filteredLines.Add(lines[i]);
                    continue;
                }

                // Continue to collapse
                if (matches) {
                    continue;
                }

                // Finish collapsing
                int collapseSize = i - collapseFrom.Value;

                filteredLines.Add(collapseSize == 1 ? Ellipsis : $"{Ellipsis}({collapseSize}/{lines.Length} lines omitted)");
                collapseFrom = null;

                filteredLines.Add(lines[i]);
            }

            if (collapseFrom.HasValue) {
                int collapseSize = lines.Length - collapseFrom.Value;
                filteredLines.Add(collapseSize == 1 ? Ellipsis : $"{Ellipsis}({collapseSize}/{lines.Length} lines omitted)");
            }

            return filteredLines.ToArray();
        }

        public static string[] CollapseLines(string[] lines, StringFilter filter, params StringFilter[] additionalFilters) {
            return CollapseLines(lines, str => additionalFilters.Prepend(filter).Any(it => it.TestFilter(str)));
        }

        /// <summary>
        /// Converts <paramref name="obj"/> - and its entries, if it is an <see cref="IEnumerable{T}"/> - into their <see cref="object.ToString"/> representations,
        /// and splits the result line-by-line via <see cref="SplitLines(string,System.StringSplitOptions)"/>.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="nullPlaceholder"></param>
        /// <returns></returns>
        public static string[] ToStringLines([CanBeNull] this object obj, [CanBeNull] string nullPlaceholder = "") {
            if (obj is IEnumerable<object> e) {
                return e.SelectMany(it => it.ToStringLines(nullPlaceholder)).SplitLines();
            }

            return obj?.ToString().SplitLines() ?? new[] { nullPlaceholder };
        }

        #endregion

        #endregion Line Management

        /// <summary>
        /// A variation on <see cref="object.ToString"/> that returns the specified <paramref name="nullPlaceholder"/> if the original <paramref name="obj"/> is <c>null</c>.
        /// </summary>
        /// <param name="obj">the original object</param>
        /// <param name="nullPlaceholder">the <see cref="string"/> returned when <paramref name="obj"/> is <c>null</c></param>
        /// <returns>the <see cref="object.ToString"/> representation of <paramref name="obj"/>, or <c>null</c></returns>
        public static string ToString([CanBeNull] this object obj, [CanBeNull] string nullPlaceholder) {
            nullPlaceholder ??= Prettification.DefaultNullPlaceholder;
            return obj == null ? nullPlaceholder : obj.ToString();
        }

        #region Lapelle deux Vid

        /// <summary>
        /// An extension method for <see cref="string.IsNullOrEmpty"/>
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        [ContractAnnotation("null => true", true)]
        public static bool IsNullOrEmpty([CanBeNull] this string str) {
            return string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// An extension method for <see cref="string.IsNullOrWhiteSpace"/>
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        [ContractAnnotation("null => true", true)]
        public static bool IsNullOrWhiteSpace([CanBeNull] this string str) {
            return string.IsNullOrWhiteSpace(str);
        }

        /// <summary>
        /// An alias for <see cref="IsNullOrWhiteSpace"/> matching Java's <a href="https://commons.apache.org/proper/commons-lang/apidocs/org/apache/commons/lang3/StringUtils.html#isBlank-java.lang.CharSequence-">StringUtils.isBlank()</a>
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        [ContractAnnotation("null => true", true)]
        public static bool IsBlank([CanBeNull] this string str) {
            return str.IsNullOrWhiteSpace();
        }

        #endregion

        #region {x}IfMissing

        public static string PrependIfMissing(this string str, string prefix) {
            return PrefixIfMissing(str, prefix);
        }

        public static string PrefixIfMissing(this string str, string prefix) {
            return str.StartsWith(prefix) ? str : str.Prefix(prefix);
        }

        public static string AppendIfMissing(this string str, string suffix) {
            return SuffixIfMissing(str, suffix);
        }

        public static string SuffixIfMissing(this string str, string suffix) {
            return str.EndsWith(suffix) ? str : str.Suffix(suffix);
        }

        #endregion

        #region Substrings

        [NotNull, Pure]
        public static string SubstringBefore([CanBeNull] this string str, [CanBeNull] string splitter) {
            if (splitter.IsNullOrEmpty()) {
                return "";
            }

            var first = str?.IndexOf(splitter, StringComparison.Ordinal);
            return first > 0 ? str.Substring(0, first.Value) : "";
        }

        [NotNull, Pure]
        public static string SubstringAfter([CanBeNull] this string str, [CanBeNull] string splitter) {
            if (str.IsNullOrEmpty() || splitter.IsNullOrEmpty()) {
                return "";
            }

            var last = str.LastIndexOf(splitter, StringComparison.Ordinal) + splitter.Length;
            return last.IsBetween(0, str.Length, Clusivity.Exclusive) ? str.Substring(last, str.Length - last) : "";
        }

        [NotNull, Pure]
        public static string SubstringBefore([CanBeNull] this string str, [NotNull] Regex pattern) {
            if (str.IsNullOrEmpty()) {
                return "";
            }

            var match = pattern.Match(str);
            return match.Success ? str.Substring(0, match.Index) : "";
        }

        [NotNull, Pure]
        public static string SubstringAfter([CanBeNull] this string str, [NotNull] Regex pattern) {
            if (str.IsNullOrEmpty()) {
                return "";
            }

            var rightToLeftPattern = new Regex(pattern.ToString(), pattern.Options | RegexOptions.RightToLeft);
            var match              = rightToLeftPattern.Match(str);
            if (match.Success) {
                // the substring starts from the END of the match
                var subStart  = match.Index + match.Length;
                var subEnd    = str.Length;
                var subLength = subEnd - subStart;
                return str.Substring(subStart, subLength);
            }
            else {
                return "";
            }
        }

        /// <summary>
        /// TODO: "Bisect" usually means "cut into two <b>equal</b> parts. I need a better name for <see cref="Bisect"/> and <see cref="BisectLast"/>.
        ///
        /// Splits <paramref name="str"/> by the <b>first</b> occurrence of <paramref name="splitter"/>.
        ///
        /// <c>null</c> is returned if <b>any</b> of the following is true:
        /// <ul>
        /// <li><b>either</b> <paramref name="str"/> or <paramref name="splitter"/> <see cref="IsNullOrEmpty"/></li>
        /// <li><paramref name="str"/> didn't contain <paramref name="splitter"/></li>
        /// </ul>
        ///
        /// </summary>
        /// <param name="str">the original <see cref="string"/></param>
        /// <param name="splitter">the <see cref="string"/> being used to split <paramref name="str"/>, which will <b>not</b> be included in the output</param>
        /// <returns></returns>
        [ContractAnnotation("str:null => null")]
        [ContractAnnotation("splitter:null => null")]
        public static (string, string)? Bisect([CanBeNull] this string str, [CanBeNull] string splitter) {
            if (str.IsNullOrEmpty() || splitter.IsNullOrEmpty()) {
                return null;
            }

            var matchStart = str.IndexOf(splitter, StringComparison.Ordinal);
            if (matchStart < 0) {
                return null;
            }

            var matchEnd = matchStart + splitter.Length;
            var before   = str.Substring(0, matchStart);
            var after    = str.Substring(matchEnd);
            return (before, after);
        }

        /// <summary>
        /// Similar to <see cref="Bisect"/>, except this splits by the <b>last</b> occurrence of <paramref name="splitter"/>.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="splitter"></param>
        /// <returns></returns>
        [ContractAnnotation("str:null => null")]
        [ContractAnnotation("splitter:null => null")]
        public static (string, string)? BisectLast([CanBeNull] this string str, [CanBeNull] string splitter) {
            if (str.IsNullOrEmpty() || splitter.IsNullOrEmpty()) {
                return null;
            }

            var matchStart = str.LastIndexOf(splitter, StringComparison.Ordinal);
            if (matchStart < 0) {
                return null;
            }

            var matchEnd = matchStart + splitter.Length;
            var before   = str.Substring(0, matchStart);
            var after    = str.Substring(matchEnd);
            return (before, after);
        }

        #endregion
    }
}