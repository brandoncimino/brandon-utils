using System.Linq;
using System.Reflection;

using BrandonUtils.Standalone.Collections;

using FowlFever.Conjugal.Affixing;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Strings.Prettifiers {
    internal static partial class InnerPretty {
        public static string PrettifyMemberInfo([NotNull] MemberInfo memberInfo, [NotNull] PrettificationSettings settings) {
            var typeName   = memberInfo.DeclaringType?.PrettifyType(settings).Suffix(".");
            var memberName = memberInfo.Name;
            return $"{typeName}{memberName}".WithTypeLabel(memberInfo.GetType(), settings);
        }

        public static string PrettifyMethodInfo([NotNull] MethodInfo methodInfo, [NotNull] PrettificationSettings settings) {
            return $"{PrettifyMemberInfo(methodInfo, settings)}({PrettifyParameters(methodInfo, settings)})";
        }

        [NotNull]
        private static string PrettifyParameters([NotNull] MethodBase methodInfo, [NotNull] PrettificationSettings settings) {
            return methodInfo.GetParameters()
                             .Select(it => PrettifyParameterInfo(it, settings))
                             .JoinString(", ");
        }

        [NotNull]
        public static string PrettifyParameterInfo([NotNull] ParameterInfo parameterInfo, [NotNull] PrettificationSettings settings) {
            return WithTypeLabel(parameterInfo.Name, parameterInfo.ParameterType, settings);
        }
    }
}