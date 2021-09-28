using System;
using System.IO;
using System.Linq;

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
        internal static readonly RegexGroup ExtensionGroup = new RegexGroup(nameof(ExtensionGroup), @"(\.[^.]+?)+$");
        internal static          char[]     Separators     = Enum.GetValues(typeof(DirectorySeparator)).Cast<DirectorySeparator>().Select(DirectorySeparatorExtensions.ToChar).ToArray();

        public static Failable ValidatePath(string maybePath) {
            Action action = () => _ = Path.GetFullPath(maybePath);
            return action.Try();
        }

        public static Failable ValidateFileName(string maybeFileName) {
            Action action = () => {
                ValidateFileNameCharacters(maybeFileName);
                _ = Path.GetFullPath(maybeFileName);
            };
            return action.Try();
        }

        private static void ValidateFileNameCharacters(string maybeFileName) {
            if (maybeFileName.ContainsAny(Path.GetInvalidFileNameChars())) {
                var badCharacters = maybeFileName.Intersect(Path.GetInvalidFileNameChars());
                throw new ArgumentException($"The string [{maybeFileName}] isn't a valid filename: it contains the illegal characters {badCharacters.Prettify()}!");
            }
        }

        public static bool IsValidPath(string maybePath) {
            return ValidatePath(maybePath).Failed == false;
        }

        public static bool IsValidFileName(string maybeFileName) {
            return ValidateFileName(maybeFileName).Failed == false;
        }


        /// <summary>
        /// This method is similar to <see cref="Path.GetExtension"/>, except that it can retrieve multiple extensions, i.e. <c>game.sav.json</c> -> <c>[.sav, .json]</c>
        /// </summary>
        /// <remarks>This uses the <see cref="ExtensionGroup"/> <see cref="RegexGroup"/> for matching.</remarks>
        /// <param name="path">a path or file name</param>
        /// <returns><b>all</b> of the extensions at the end of the <paramref name="path"/></returns>
        public static string[] GetExtensions(string path) {
            return GetFullExtension(path)
                   .Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries)
                   .Select(it => it.PrependIfMissing("."))
                   .ToArray();
        }

        public static string GetFullExtension(string path) {
            return Path.GetFileName(path)
                       .Match(ExtensionGroup.Regex)
                       .Groups[ExtensionGroup.GroupName]
                       .Value;
        }

        public static string GetFileNameWithoutExtensions(string path) {
            var fileName    = Path.GetFileName(path);
            var firstPeriod = fileName.IndexOf(".", StringComparison.Ordinal);
            return firstPeriod < 0 ? fileName : fileName.Substring(0, firstPeriod);
        }

        [ContractAnnotation("null => false")]
        public static bool EndsWithSeparator([CanBeNull] string path) {
            path = path?.TrimEnd();
            return path != null && Separators.Any(it => path.EndsWith(it.ToString()));
        }

        [ContractAnnotation("null => false")]
        public static bool StartsWithSeparator([CanBeNull] string path) {
            path = path?.TrimStart();
            return path != null && Separators.Any(it => path.StartsWith(it.ToString()));
        }

        public static string EnsureTrailingSeparator(string path, DirectorySeparator separator = DirectorySeparator.Universal) {
            path = NormalizeSeparators(path, separator);
            return EndsWithSeparator(path) ? path : path.TrimEnd().Suffix(separator.ToChar().ToString());
        }

        public static string StripLeadingSeparator(string path, DirectorySeparator separator = DirectorySeparator.Universal) {
            path = NormalizeSeparators(path, separator);
            return StartsWithSeparator(path) ? path.TrimStart().Prefix(separator.ToChar().ToString()) : path;
        }

        public static string NormalizeSeparators(string path, DirectorySeparator separator = DirectorySeparator.Universal) {
            return Separators.Aggregate(
                path.Trim(),
                (current, c) => current.Replace(c, separator.ToChar())
            );
        }
    }
}