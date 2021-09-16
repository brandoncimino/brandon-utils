using System.Text.RegularExpressions;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Strings {
    /// <summary>
    /// Keeps a <see cref="GroupName"/> and <see cref="GroupPattern"/> together and formats them nicely.
    /// </summary>
    /// <example>
    /// <ul>
    /// <li><see cref="GroupName"/> = <c>"phoneNumber";</c></li>
    /// <li><see cref="GroupPattern"/> = new <see cref="Regex"/><c>(@"\d{3}-\d{3}-\d{4}");</c></li>
    /// <li><see cref="GroupRegex"/> => <c><![CDATA[(?<phoneNumber>\d{3}-\d{3}-\d{4})]]></c></li>
    /// </ul>
    /// </example>
    [PublicAPI]
    public class RegexGroup {
        public readonly string GroupName;
        [RegexPattern]
        public readonly Regex GroupPattern;
        [RegexPattern]
        public Regex GroupRegex => new Regex($@"(?<{GroupName}>{GroupPattern})");

        public RegexGroup(string groupName, [RegexPattern] string groupPattern) : this(groupName, new Regex(groupPattern)) { }

        public RegexGroup(string groupName, [RegexPattern] Regex groupPattern) {
            GroupName    = groupName;
            GroupPattern = groupPattern;
        }

        public override string ToString() {
            return GroupRegex.ToString();
        }
    }
}