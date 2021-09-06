using System.Reflection;

namespace BrandonUtils.Standalone.Strings.Prettifiers {
    internal static partial class InnerPretty {
        public static string PrettifyMemberInfo(MemberInfo memberInfo, PrettificationSettings settings = default) {
            var str = $"{memberInfo.DeclaringType}.{memberInfo.Name}";

            if (settings?.Flags.HasFlag(PrettificationFlags.IncludeTypeLabels) == true) {
                str = $"[{memberInfo.GetType().PrettifyType()}]{str}";
            }

            return str;
        }
    }
}