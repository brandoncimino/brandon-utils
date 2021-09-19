using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using BrandonUtils.Standalone.Collections;
using BrandonUtils.Standalone.Strings;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Clerical.Saving {
    /// <summary>
    /// A builder for names of <see cref="SaveFile{TData}"/>s.
    /// </summary>
    internal class SaveFileName {
        private string _nickname;
        public string Nickname {
            get => _nickname;
            set => _nickname = Validate_Nickname(value);
        }
        public DateTime TimeStamp;

        public const string DefaultExtension = ".sav.json";
        private      string _fullExtension   = DefaultExtension;

        public string FullExtension {
            get => _fullExtension;
            set => _fullExtension = value.PrefixIfMissing(".");
        }

        public string BaseName => $"{Nickname}_{TimeStamp.Ticks}";
        public string Rendered => $"{BaseName}{FullExtension}";

        private static readonly  RegexGroup NicknameGroup       = new RegexGroup(nameof(NicknameGroup),  @".+");
        private static readonly  RegexGroup TimeStampGroup      = new RegexGroup(nameof(TimeStampGroup), @"\d+");
        internal static readonly Regex      BaseFileNamePattern = new Regex($@"^{NicknameGroup}_{TimeStampGroup}$");

        public static SaveFileName Parse(string fileName) {
            var parsed   = new SaveFileName();
            var baseName = BPath.GetFileNameWithoutExtensions(fileName);
            var match    = BaseFileNamePattern.Match(baseName);
            if (match.Success) {
                parsed.Nickname = match.Groups[NicknameGroup.GroupName].Value;

                var tsString = match.Groups[TimeStampGroup.GroupName].Value;
                parsed.TimeStamp = new DateTime(long.Parse(tsString));
            }

            parsed.FullExtension = BPath.GetFullExtension(fileName);
            return parsed;
        }

        public static SaveFileName Parse(FileInfo fileInfo) {
            return Parse(fileInfo.Name);
        }

        [ContractAnnotation("null => stop")]
        private static string Validate_Nickname([CanBeNull] string nickname) {
            if (nickname.IsNullOrEmpty()) {
                throw new ArgumentNullException(nameof(nickname), $"Must not be null or empty, but was [{nickname}]!");
            }

            var badCharacters = nickname.ToCharArray().Intersect(Path.GetInvalidFileNameChars()).ToList();
            if (badCharacters.IsNotEmpty()) {
                throw new ArgumentException($"{nameof(nickname)} [{nickname}] contained invalid file name characters: {badCharacters.Prettify()}");
            }

            return nickname;
        }
    }
}