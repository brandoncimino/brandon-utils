using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using BrandonUtils.Standalone.Collections;
using BrandonUtils.Standalone.Optional;
using BrandonUtils.Standalone.Strings;

using FowlFever.Conjugal.Affixing;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Clerical {
    /// <summary>
    /// TODO: There is almost certainly a cross-platform library to use for this that I can get from Nuget
    /// </summary>
    [PublicAPI]
    public static class BPath {
        internal static readonly RegexGroup ExtensionGroup            = new RegexGroup(nameof(ExtensionGroup), @"(\.[^.]+?)+$");
        internal static readonly char[]     Separators                = Enum.GetValues(typeof(DirectorySeparator)).Cast<DirectorySeparator>().Select(DirectorySeparatorExtensions.ToChar).ToArray();
        internal static readonly Regex      DirectorySeparatorPattern = new Regex(@"[\\\/]");
        internal static readonly string     OpenFolderIcon            = "📂";
        internal static readonly string     ClosedFolderIcon          = "📁";
        internal static readonly string     FileIcon                  = "📄";

        public static Failable ValidatePath([CanBeNull] string maybePath) {
            Action action = () => _ = Path.GetFullPath(maybePath!);
            return action.Try();
        }

        public static Failable ValidateFileName([CanBeNull] string maybeFileName) {
            Action action = () => {
                ValidateFileNameCharacters(maybeFileName);
                _ = Path.GetFullPath(maybeFileName!);
            };
            return action.Try();
        }

        [ContractAnnotation("null => stop")]
        private static void ValidateFileNameCharacters([CanBeNull] string maybeFileName) {
            if (maybeFileName == null) {
                throw new ArgumentNullException($"The string [{maybeFileName.ToString(Prettification.DefaultNullPlaceholder)}] wasn't a valid filename: it was blank!");
            }

            if (maybeFileName.ContainsAny(Path.GetInvalidFileNameChars())) {
                var badCharacters = maybeFileName.Intersect(Path.GetInvalidFileNameChars());
                throw new ArgumentException($"The string [{maybeFileName}] isn't a valid filename: it contains the illegal characters {badCharacters.Prettify()}!");
            }
        }

        [ContractAnnotation("null => false")]
        public static bool IsValidPath([CanBeNull] string maybePath) {
            return ValidatePath(maybePath).Failed == false;
        }

        [ContractAnnotation("null => false")]
        public static bool IsValidFileName([CanBeNull] string maybeFileName) {
            return ValidateFileName(maybeFileName).Failed == false;
        }


        /// <summary>
        /// This method is similar to <see cref="Path.GetExtension"/>, except that it can retrieve multiple extensions, i.e. <c>game.sav.json</c> -> <c>[.sav, .json]</c>
        /// </summary>
        /// <remarks>This uses the <see cref="ExtensionGroup"/> <see cref="RegexGroup"/> for matching.</remarks>
        /// <param name="path">a path or file name</param>
        /// <returns><b>all</b> of the extensions at the end of the <paramref name="path"/></returns>
        [CanBeNull]
        [ContractAnnotation("path:null => null")]
        public static string[] GetExtensions([CanBeNull] string path) {
            if (path == null) {
                return null;
            }

            return GetFullExtension(path)
                   .Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries)
                   .Select(it => it.PrependIfMissing("."))
                   .ToArray();
        }

        [CanBeNull]
        [ContractAnnotation("path:null => null")]
        public static string GetFullExtension([CanBeNull] string path) {
            if (path == null) {
                return null;
            }

            return Path.GetFileName(path)
                       .Match(ExtensionGroup.Regex)
                       .Groups[ExtensionGroup.GroupName]
                       .Value;
        }

        [CanBeNull]
        [ContractAnnotation("path:null => null")]
        public static string GetFileNameWithoutExtensions([CanBeNull] string path) {
            if (path == null) {
                return null;
            }

            var fileName    = Path.GetFileName(path);
            var firstPeriod = fileName.IndexOf(".", StringComparison.Ordinal);
            return firstPeriod < 0 ? fileName : fileName.Substring(0, firstPeriod);
        }

        [ContractAnnotation("null => false")]
        public static bool EndsWithSeparator([CanBeNull] string path) {
            return path.EndsWith(DirectorySeparatorPattern);
        }

        [ContractAnnotation("null => false")]
        public static bool StartsWithSeparator([CanBeNull] string path) {
            return path.StartsWith(DirectorySeparatorPattern);
        }

        [NotNull]
        public static string EnsureTrailingSeparator([CanBeNull] string path, DirectorySeparator separator = DirectorySeparator.Universal) {
            if (path == null) {
                return separator.ToCharString();
            }

            return NormalizeSeparators(
                path?.Trim()
                    .TrimEnd(DirectorySeparatorPattern)
                    .Suffix(separator.ToCharString()),
                separator
            );
        }

        [NotNull]
        public static string StripLeadingSeparator([CanBeNull] string path, DirectorySeparator separator = DirectorySeparator.Universal) {
            if (path == null) {
                return "";
            }

            return NormalizeSeparators(
                path?.Trim()
                    .TrimStart(separator.ToCharString()),
                separator
            );
        }

        [NotNull]
        public static string NormalizeSeparators([CanBeNull] string path, DirectorySeparator separator = DirectorySeparator.Universal) {
            return path.IsBlank() ? "" : DirectorySeparatorPattern.Replace(path.Trim(), separator.ToCharString());
        }

        [NotNull]
        public static string JoinPath(
            [CanBeNull] string parent,
            [CanBeNull] string child
        ) {
            return JoinPath(parent, child, default(DirectorySeparator));
        }

        [NotNull]
        public static string JoinPath(
            [CanBeNull] string parent,
            [CanBeNull] string child,
            DirectorySeparator separator
        ) {
            parent = parent?.Trim().TrimEnd(DirectorySeparatorPattern);
            child  = child?.Trim().TrimStart(DirectorySeparatorPattern);
            var path = parent.JoinWith(child, separator.ToCharString());
            return NormalizeSeparators(path, separator);
        }

        [NotNull]
        public static string JoinPath(
            [CanBeNull] DirectoryInfo parentDirectory,
            [CanBeNull] string        child,
            DirectorySeparator        separator = DirectorySeparator.Universal
        ) {
            return JoinPath(parentDirectory?.FullName, child, separator);
        }

        [NotNull]
        public static string JoinPath([CanBeNull, ItemCanBeNull] params string[] parts) {
            return JoinPath(parts, default(DirectorySeparator));
        }

        [NotNull]
        public static string JoinPath([CanBeNull, ItemCanBeNull] IEnumerable<string> parts, DirectorySeparator separator = DirectorySeparator.Universal) {
            return parts?.Aggregate((pathSoFar, nextPart) => JoinPath(pathSoFar, nextPart, separator)) ?? "";
        }
    }
}