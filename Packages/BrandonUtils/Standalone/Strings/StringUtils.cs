using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

using BrandonUtils.Standalone.Collections;

using Newtonsoft.Json;

// using UnityEngine;

namespace BrandonUtils.Standalone.Strings {
    public static class StringUtils {
        /// <summary>
        ///     Prepends <paramref name="toIndent" />.
        /// </summary>
        /// <param name="toIndent">The <see cref="string" /> to be indented.</param>
        /// <param name="indentCount">The number of indentations (i.e. number of times hitting "tab").</param>
        /// <param name="indentSize">The size of each individual indentation.</param>
        /// <param name="indentChar">The character to use for indentation.</param>
        /// <returns></returns>
        public static string Indent(this string toIndent, int indentCount = 1, int indentSize = 2, char indentChar = ' ') {
            return indentChar.ToString().Repeat(indentSize).Repeat(indentCount) + toIndent;
        }

        /// <summary>
        ///     Joins <paramref name="toRepeat" /> with itself <paramref name="repetitions" /> times, using the optional <paramref name="separator" />
        /// </summary>
        /// <param name="toRepeat">The <see cref="string" /> to be joined with itself.</param>
        /// <param name="repetitions">The number of times <paramref name="toRepeat" /> should be repeated.</param>
        /// <param name="separator">An optional character, analogous to </param>
        /// <returns></returns>
        public static string Repeat(this string toRepeat, int repetitions, string separator = "") {
            var list = new List<string>();
            for (var i = 0; i < repetitions; i++) {
                list.Add(toRepeat);
            }

            return string.Join(separator, list);
        }

        /// <inheritdoc cref="Repeat(string,int,string)" />
        public static string Repeat(this char toRepeat, int repetitions, string separator = "") {
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

        public static string Prettify(object thing, bool recursive = true, int recursionCount = 0) {
            const int recursionMax = 10;
            var       type         = thing.GetType().ToString();
            string    method       = null;
            string    prettyString = null;

            switch (thing) {
                //don't do anything special with strings
                //check for value types (int, char, etc.), which we shouldn't do anything fancy with
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
            prettyString = prettyString ?? thing.ToString();
            method       = method ?? "NO METHOD FOUND";


            return $"[{method}]{prettyString}".Indent(indentCount: recursionCount);
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

        public static string[] SplitLines(this string contentWithLines) {
            return Splitex(contentWithLines, "[\n\r]+");
        }

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
                          .Append($"...[{lns.Length - lineCount} lines omitted]")
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

                filteredLines.Add(collapseSize == 1 ? "..." : $"...({collapseSize}/{lines.Length} lines omitted)");
                collapseFrom = null;

                filteredLines.Add(lines[i]);
            }

            if (collapseFrom.HasValue) {
                int collapseSize = lines.Length - collapseFrom.Value;
                filteredLines.Add(collapseSize == 1 ? "..." : $"...({collapseSize}/{lines.Length} lines omitted)");
            }

            return filteredLines.ToArray();
        }

        public static string[] CollapseLines(string[] lines, StringFilter filter, params StringFilter[] additionalFilters) {
            return CollapseLines(lines, str => additionalFilters.Prepend(filter).Any(it => it.TestFilter(str)));
        }

        #endregion
    }
}