using System;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Strings.Prettifiers {
    internal partial class InnerPretty {
        [NotNull]
        internal static string PrettifyEnum([NotNull] Enum enm, [NotNull] PrettificationSettings settings) {
            settings = Prettification.ResolveSettings(settings);
            return enm.ToString().WithTypeLabel(enm.GetType(), settings, ".");
        }
    }
}