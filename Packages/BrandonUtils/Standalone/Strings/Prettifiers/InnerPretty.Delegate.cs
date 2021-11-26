using System;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Strings.Prettifiers {
    internal partial class InnerPretty {
        [NotNull]
        public static string PrettifyDelegate([NotNull] Delegate del, [CanBeNull] PrettificationSettings settings = default) {
            return PrettifyMethodInfo(del.Method);
        }
    }
}